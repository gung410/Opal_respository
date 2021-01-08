import { Gulp } from 'gulp';
import { ThunderNodeBuildRig } from './node/ThunderNodeBuildRig';
import { ThunderWebBuildRig } from './web/ThunderWebBuildRig';

/**
 * @public
 */
const webRig: ThunderWebBuildRig = new ThunderWebBuildRig();

/**
 * @public
 */
const nodeRig: ThunderNodeBuildRig = new ThunderNodeBuildRig();

/**
 * Initialize ThunderWebBuildRig
 * @public
 * @param gulp - Gulp instance
 * @returns - ThunderWebBuildRig instance
 */
function initializeWebBuildRig(gulp: Gulp): ThunderWebBuildRig {
  webRig.initialize(gulp);
  return webRig;
}

/**
 * Initialize ThunderNodeBuildRig
 * @public
 * @param gulp - Gulp instance
 * @returns - ThunderWebBuildRig instance
 */
function initializeNodeBuildRig(gulp: Gulp): ThunderWebBuildRig {
  nodeRig.initialize(gulp);
  return webRig;
}

export * from '@microsoft/sp-build-common';
export * from '@microsoft/sp-build-core-tasks';
export * from './ThunderTask';
export * from './web/ThunderWebBuildRigTask';
export * from './web/ThunderWebBuildRig';
export * from './node/ThunderNodeBuildRigTask';
export * from './node/ThunderNodeBuildRig';
export { initializeWebBuildRig, webRig, initializeNodeBuildRig, nodeRig };
