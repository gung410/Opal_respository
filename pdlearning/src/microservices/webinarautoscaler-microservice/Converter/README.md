# Webinar Video converter

The microservice for management webinar meeting records.

## How to use?

- Build

    ```cmd
        docker build -t bbb-converter ./Converter
    ```

- Run

    ```cmd
        docker run -v %cd%/meeting-records:/var/www/bigbluebutton-default/record/ bbb-converter YOUR_MEETING_PLAYBACK_URL  MEETING_ID 0 true
    ```
