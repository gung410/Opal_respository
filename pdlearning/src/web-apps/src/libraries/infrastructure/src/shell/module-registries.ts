import { ModuleInfo, ModuleInstance } from './shell.models';

export class ModuleInstanceRegistry {
  private registry: ModuleInstance[] = [];
  private lastModuleInstanceId: number = 1;

  public register(moduleInstance: ModuleInstance): void {
    moduleInstance.id = this.generateModuleInstanceId();

    this.registry.push(moduleInstance);
  }

  public get(moduleInstanceId: string): ModuleInstance | null {
    return this.registry.find(instance => instance.id === moduleInstanceId);
  }

  public getModuleByInfoId(moduleInfoId: string): ModuleInstance | null {
    return this.registry.find(instance => instance.moduleInfo && instance.moduleInfo.id === moduleInfoId);
  }

  public remove(moduleInstanceId: string): void {
    const index: number = this.registry.findIndex(m => m.id === moduleInstanceId);

    this.registry.splice(index, 1);
  }

  public getLastModuleInstance(): ModuleInstance | null {
    if (this.registry.length > 0) {
      return this.registry[this.registry.length - 1];
    }

    return null;
  }

  public getReversedModuleInstances(): ModuleInstance[] {
    return this.registry.slice().reverse();
  }

  private generateModuleInstanceId(): string {
    return `module-instance-${this.lastModuleInstanceId++}`;
  }
}

export class ModuleInfoRegistry {
  private registry: ModuleInfo[] = [];

  public register(moduleInfo: ModuleInfo): void {
    this.registry.push(moduleInfo);
  }

  public get(moduleInfoId: string): ModuleInfo | undefined {
    return this.registry.find(instance => instance.id === moduleInfoId);
  }

  public any(moduleInfoId: string): boolean {
    return this.registry.some(instance => instance.id === moduleInfoId);
  }

  public remove(moduleInfoId: string): void {
    const index: number = this.registry.findIndex(m => m.id === moduleInfoId);

    this.registry.splice(index, 1);
  }
}
