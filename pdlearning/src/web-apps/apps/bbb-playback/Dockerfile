FROM node:10.19.0-alpine AS builder

# Build playback
ARG PLAYBACK_URL=/playback/presentation/2.3/

# Copy source
WORKDIR /workspace
COPY . .
RUN echo SKIP_PREFLIGHT_CHECK=true  >> .env
RUN echo PUBLIC_URL=$PLAYBACK_URL   >> .env

# Debug
RUN cat .env

# Build app
RUN npm   install -g -f yarn
RUN yarn  install
RUN yarn  build

# Build a small nginx image with static website
FROM    nginx:alpine
ARG     BBB_PLAYBACK_DIR=/var/bigbluebutton/playback/presentation/2.3
VOLUME  [ "$BBB_PLAYBACK_DIR" ]
RUN     rm -rf  $BBB_PLAYBACK_DIR/*

# From 'builder' stage copy over the artifacts in dist folder to default nginx public folder
COPY    --from=builder /workspace/build $BBB_PLAYBACK_DIR
RUN     chown -R nginx:nginx $BBB_PLAYBACK_DIR

# Configure nginx
COPY    ./bbb-playback.nginx /etc/nginx/conf.d/bbb-playback.conf
RUN     mv /etc/nginx/conf.d/default.conf /etc/nginx/conf.d/default.conf.bk

EXPOSE  80
CMD     ["nginx", "-g", "daemon off;"]

