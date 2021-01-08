### STAGE 1: Build ###

# We label our stage as ‘builder’
FROM node:12.2.0 as builder
ARG env=prod


COPY .npmrc ./
COPY package.json ./

## Storing node modules on a separate layer will prevent unnecessary npm installs at each build

RUN npm install && mkdir /ng-app && mv ./node_modules ./ng-app

WORKDIR /ng-app

COPY . .

## Build the angular app in production mode and store the artifacts in dist folder
ENV NODE_OPTIONS "--max-old-space-size=8192"
RUN npm run build:$env



# Build a small nginx image with static website
FROM nginx:alpine
RUN rm -rf /usr/share/nginx/html/*
COPY nginx.conf /etc/nginx/nginx.conf
## From ‘builder’ stage copy over the artifacts in dist folder to default nginx public folder
COPY --from=builder /ng-app/dist /usr/share/nginx/html
RUN ls -la /usr/share/nginx/html
RUN chown -R nginx:nginx /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]

