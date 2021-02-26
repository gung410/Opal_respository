import { NestedTreeControl } from '@angular/cdk/tree';
import { Component, Inject, Input, OnDestroy, OnInit } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatTreeNestedDataSource } from '@angular/material/tree';
import { Utils } from 'app-utilities/utils';
import { ServiceSchemeCodeEnum } from 'app/shared/constants/service-scheme.enum';
import {
  AED_METADATA_TYPE,
  EO_METADATA_TYPE
} from 'app/taxonomy-management/constant/metadata-type-info.const';
import { MetadataTagModel } from 'app/taxonomy-management/models/metadata-tag.model';
import { MetadataTypeNode } from 'app/taxonomy-management/models/metadata-type-node.model';
import {
  EAS_METADATA_TYPE,
  MKE_METADATA_TYPE
} from '../../constant/metadata-type-info.const';

@Component({
  selector: 'metadata-type-dialog',
  templateUrl: './metadata-type-dialog.component.html',
  styleUrls: ['./metadata-type-dialog.component.scss']
})
export class MetadataTypeDialogComponent implements OnInit, OnDestroy {
  get selectedMetadataType(): MetadataTypeNode {
    return this._selectedMetadataType;
  }
  @Input() set selectedMetadataType(selectedMetadataType: MetadataTypeNode) {
    if (selectedMetadataType == null) {
      return;
    }

    this._selectedMetadataType = selectedMetadataType;
  }

  treeControl: NestedTreeControl<MetadataTypeNode> = new NestedTreeControl<MetadataTypeNode>(
    (node) => node.children
  );
  dataSource: MatTreeNestedDataSource<MetadataTypeNode> = new MatTreeNestedDataSource<MetadataTypeNode>();

  private _selectedMetadataType: MetadataTypeNode;
  constructor(
    public dialogRef: MatDialogRef<any>,
    @Inject(MAT_DIALOG_DATA) public serviceSchemeInfo: MetadataTagModel
  ) {
    this.updateSourceTree(serviceSchemeInfo);
  }

  onCloseBtnClicked(): void {
    this.dialogRef.close({
      metadataType: this.selectedMetadataType
    });
  }
  ngOnInit(): void {
    this.expandNode();
  }
  ngOnDestroy(): void {
    // throw new Error('Method not implemented.');
  }

  hasChild = (_: number, node: MetadataTypeNode) =>
    !!node.children && node.children.length > 0;

  private updateSourceTree(serviceSchemeInfo: MetadataTagModel): void {
    if (serviceSchemeInfo == null) {
      return;
    }

    switch (serviceSchemeInfo.codingScheme) {
      case ServiceSchemeCodeEnum.AED:
        this.dataSource.data = AED_METADATA_TYPE;
        break;
      case ServiceSchemeCodeEnum.EAS:
        this.dataSource.data = EAS_METADATA_TYPE;
        break;
      case ServiceSchemeCodeEnum.EO:
        this.dataSource.data = EO_METADATA_TYPE;
        break;
      case ServiceSchemeCodeEnum.MKE:
        this.dataSource.data = MKE_METADATA_TYPE;
        break;
      default:
        this.dataSource.data = EO_METADATA_TYPE;
        break;
    }
  }

  private expandNode(): void {
    if (this.selectedMetadataType == null) {
      return;
    }

    const metadataTypeDic = Utils.toDictionary(
      this.dataSource.data,
      (node) => node.nodeId
    );

    if (this.selectedMetadataType.parentNodeId) {
      this.treeControl.expand(
        metadataTypeDic[this.selectedMetadataType.parentNodeId]
      );

      this.treeControl.expand(
        metadataTypeDic[this.selectedMetadataType.nodeId]
      );
    }
  }
}
