FROM  node

ARG RECORD_DIR=/data/records

WORKDIR /root

# Update Software Repository
RUN mkdir -p /home/node/app/node_modules && chown -R node:node /home/node/app
# RUN apt-get -y curl
RUN curl -sS -o - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add
RUN echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" > /etc/apt/sources.list.d/google-chrome.list
RUN apt-get -y update
RUN apt-get -y install google-chrome-stable software-properties-common ffmpeg
RUN apt -y install xvfb

# Install export app
RUN mkdir /root/bbb-recorder
COPY . /root/bbb-recorder
WORKDIR /root/bbb-recorder
RUN npm install --ignore-scripts

# Run export
VOLUME  [ "$RECORD_DIR" ]

RUN echo copyToPath=$RECORD_DIR >> .env

ENTRYPOINT yarn export $MEETING_URL MEETING_ID 0 true
