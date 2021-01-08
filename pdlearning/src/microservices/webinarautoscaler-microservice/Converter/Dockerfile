FROM  node


WORKDIR /root

# Update Software Repository
RUN mkdir -p /home/node/app/node_modules && chown -R node:node /home/node/app
# RUN apt-get -y curl
RUN curl -sS -o - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add
RUN echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" > /etc/apt/sources.list.d/google-chrome.list
RUN apt-get -y update
RUN apt-get -y install google-chrome-stable
RUN apt-get -y install software-properties-common
RUN add-apt-repository ppa:jonathonf/ffmpeg-4
# RUN apt-get -y update
RUN apt-get -y install ffmpeg
RUN apt -y install xvfb

# Install export app
RUN git clone https://github.com/TatChu/bbb-recorder.git
RUN cd bbb-recorder && npm install --ignore-scripts
WORKDIR /root/bbb-recorder

# Run export
VOLUME [ "/mnt/efs/records" ]

RUN echo copyToPath=/mnt/efs/records >> .env
RUN echo playbackFile=playback.html >> .env

ENV copyToPath=/mnt/efs/records
ENTRYPOINT npm run export $MEETING_URL MEETING_ID 0 true
