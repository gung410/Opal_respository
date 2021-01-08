FROM node:10.19.0-alpine as builder
ARG env=production

# Install yarn.
RUN npm install -g -f yarn

# Prepare dependencies.
WORKDIR /workspace
COPY .npmrc /workspace
COPY gulpfile.js /workspace
COPY lerna.json /workspace
COPY package.json /workspace
COPY tsconfig-node.json /workspace
COPY tslint.json /workspace
COPY yarn.lock /workspace
COPY /src/package.json /workspace/src/package.json
COPY /scripts/. /workspace/scripts/
COPY /tools/. /workspace/tools/
COPY /src/toolkit/. /workspace/src/toolkit/
# --ignore-monorepo-tasks to ignore git/githooks stuffs
RUN yarn install --ignore-monorepo-tasks

# Build the angular app.
COPY . .
WORKDIR /workspace/src
RUN yarn build:$env

# Build a small nginx image with static website
FROM nginx:alpine
RUN rm -rf /usr/share/nginx/html/*
COPY nginx.conf /etc/nginx/nginx.conf

# From 'builder' stage copy over the artifacts in dist folder to default nginx public folder
COPY --from=builder /workspace/src/dist/opal20 /usr/share/nginx/html
RUN ls -la /usr/share/nginx/html
RUN chown -R nginx:nginx /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]

