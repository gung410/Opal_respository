const server = require('json-server');
const color = require('ansi-colors');
const jph = require('json-parse-helpfulerror');
const enableDestroy = require('server-destroy');
const app = server.create();
const middlewares = server.defaults();
const path = require('path');
const fs = require('fs');
const glob = require('glob');
const port = 3000;
let serverInstance;
let apiMap = {};
let readError = false;

glob.sync('{,!(node_modules)/}**/*api.json').forEach(file => {
  const source = path.join(process.cwd(), file);

  apiMap = Object.assign(apiMap, require(path.join(process.cwd(), file)));

  fs.watchFile(source, () => {
    let obj;
    try {
      obj = jph.parse(fs.readFileSync(source));
      if (readError) {
        console.log(color.cyan(`Read error has been fixed :)`));
        readError = false;
      }
    } catch (e) {
      readError = true;
      console.error(e.message);
      return;
    }
    apiMap = Object.assign(apiMap, obj);

    console.log(source);

    if (serverInstance) {
      serverInstance.destroy();
    }

    console.log(color.gray(`${source} has changed. Reloading...`));

    startServer();
  });
});

startServer();

function startServer() {
  const router = server.router(apiMap);

  router.render = (req, res) => {
    res.jsonp(res.locals.data.response);
  };

  app.use(middlewares);
  app.use(server.bodyParser);
  app.use((req, res, next) => {
    if (req.method === 'POST') {
      req.method = 'GET';
      req.query = req.body;
    }
    next();
  });
  app.use(
    server.rewriter({
      '/:resource/:id/full-with-questions-by-id': '/:resource/:id'
    })
  );
  app.use(router);
  serverInstance = app.listen(port, () => {
    console.log(color.green(`JSON Server is running on http://localhost:${port}`));
  });

  enableDestroy(serverInstance);
}
