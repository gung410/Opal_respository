const puppeteer = require("puppeteer");
const Xvfb = require("xvfb");
const fs = require("fs");
const os = require("os");
const homedir = os.homedir();
const platform = os.platform();
const { copyToPath } = require("./env");
const spawn = require("child_process").spawn;

var xvfb = new Xvfb({
  silent: true,
  xvfb_args: [
    "-screen",
    "0",
    "1920x1080x24",
    "-ac",
    "-nolisten",
    "tcp",
    "-dpi",
    "96",
    "+extension",
    "RANDR",
  ],
});
// TODO: allow custom w/h from env variable
var width = 1920;
var height = 1080;
var options = {
  headless: false,
  args: [
    "--enable-usermedia-screen-capturing",
    "--allow-http-screen-capture",
    "--auto-select-desktop-capture-source=bbbrecorder",
    "--load-extension=" + __dirname,
    "--disable-extensions-except=" + __dirname,
    "--disable-infobars",
    "--no-sandbox",
    "--shm-size=1gb",
    "--disable-dev-shm-usage",
    "--start-fullscreen",
    "--app=https://www.google.com/",
    `--window-size=${width},${height}`,
  ],
};

if (platform == "linux") {
  options.executablePath = "/usr/bin/google-chrome";
} else if (platform == "darwin") {
  options.executablePath =
    "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome";
}

async function main() {
  let browser, page;

  try {
    if (platform == "linux") {
      xvfb.startSync();
    }

    var url = process.argv[2];
    if (!url) {
      console.warn("URL undefined!");
      process.exit(1);
    }
    // Verify if recording URL has the correct format
    // URL playback like: http://BBB_HOST/playback/presentation/2.3/618dcdfb0cd9ae4481164961c4796dd8e3930c8d-1606268814166
    var urlRegex = /\/playback\/presentation\/2\.3\/[a-z0-9]{40}-[0-9]{13}/g;
    if (!urlRegex.test(url)) {
      console.warn("Invalid recording URL!");
      process.exit(1);
    }

    var exportname = process.argv[3];
    // Use meeting ID as export name if it isn't defined or if its value is "MEETING_ID"
    if (!exportname || exportname == "MEETING_ID") {
      exportname = url.split("/").pop() + ".webm";
      console.log("exporting as ", exportname);
    }

    var duration = process.argv[4];

    // If duration isn't defined, set it in 0
    if (!duration) {
      duration = 0;
      // Check if duration is a natural number
    } else if (!Number.isInteger(Number(duration)) || duration < 0) {
      console.warn("Duration must be a natural number!");
      process.exit(1);
    }

    var convert = process.argv[5];
    if (!convert) {
      convert = false;
    } else if (convert !== "true" && convert !== "false") {
      console.warn("Invalid convert value!");
      process.exit(1);
    }

    browser = await puppeteer.launch(options);
    const pages = await browser.pages();

    page = pages[0];

    page.on("console", (msg) => {
      var m = msg.text();
      console.log("\tLog from console:", m);
    });

    await page._client.send("Emulation.clearDeviceMetricsOverride");
    // Catch URL unreachable error
    await page.goto(url, { waitUntil: "networkidle2" }).catch((e) => {
      console.error("\tRecording URL unreachable!");
      process.exit(2);
    });
    await page.setBypassCSP(true);

    console.log("---->>>> Checking error-----------------------------------");
    // Check if recording exists by check error code
    var errorCode = await page.evaluate(() => {
      let errorElement = document.getElementById("error");

      return errorElement ? errorElement.textContent : 0;
    });

    if (errorCode) {
      console.warn("\tError code: " + errorCode);
      process.exit(errorCode);
    }

    console.log("---->>>> Getting record duration--------------------------");

    await page.waitForSelector("#vjs_video_3_html5_api", {
      timeout: 5 * 60 * 1000, // 5 mins
    });

    console.log("---->>>> Detecting record duration------------------------");
    let recDuration = await page.evaluate(() => {
      return document.getElementById("vjs_video_3_html5_api").duration;
    });

    // If duration was set to 0 or is greater than recDuration, use recDuration value
    if (duration == 0 || duration > recDuration) {
      duration = recDuration;
    }

    console.log("\tRecord duration: " + duration + " (s)");

    console.log("---->>>> Hiding controls----------------------------------");
    await page.waitForSelector("button[title='Play']");
    await page.$eval(
      'button[aria-label="Fullscreen content"]',
      (element) => (element.style.display = "none")
    );
    await page.$eval(
      'button[aria-label="Search"]',
      (element) => (element.style.display = "none")
    );
    await page.$eval(
      'button[aria-label="Swap content"]',
      (element) => (element.style.display = "none")
    );
    await page.$eval(
      ".bottom-content",
      (element) => (element.style.display = "none")
    );
    await page.$eval(".top-bar", (element) => (element.style.display = "none"));
    await page.$eval(
      ".vjs-control-bar",
      (element) => (element.style.opacity = "0")
    );

    await page.click("button[title='Play']", {
      waitUntil: "domcontentloaded",
    });

    await page.evaluate((x) => {
      console.log("\tREC_START");
      window.postMessage({ type: "REC_START" }, "*");
    });

    console.log("---->>>> Start recording----------------------------------");
    // Perform any actions that have to be captured in the exported video
    await page.waitFor(duration * 1000);

    await page.evaluate((filename) => {
      window.postMessage({ type: "SET_EXPORT_PATH", filename: filename }, "*");
      console.log("\tREC_STOP");
      window.postMessage({ type: "REC_STOP" }, "*");
    }, exportname);

    console.log("---->>>> Start downloading--------------------------------");
    // Wait for download of webm to complete
    await page.waitForSelector("html.downloadComplete", {
      timeout: 15 * 60 * 1000,
    });

    console.log("---->>>> Start converting---------------------------------");
    if (convert) {
      convertAndCopy(exportname);
    } else {
      copyOnly(exportname);
    }
  } catch (err) {
    console.log(err);
  } finally {
    page.close && (await page.close());
    browser.close && (await browser.close());

    if (platform == "linux") {
      xvfb.stopSync();
    }
  }
}

function convertAndCopy(filename) {
  var copyFromPath = homedir + "/Downloads";
  var onlyfileName = filename.split(".webm");
  var mp4File = onlyfileName[0] + ".mp4";
  var copyFrom = copyFromPath + "/" + filename + "";
  var copyTo = copyToPath + "/" + mp4File;

  if (!fs.existsSync(copyToPath)) {
    fs.mkdirSync(copyToPath);
  }
  console.log("---->>>> Copying video file---------------------------------");
  console.log("\t- From: '" + copyFrom + "'");
  console.log("\t- To: '" + copyTo + "'");

  console.log("---->>>> Converting video file------------------------------");
  const ls = spawn(
    "ffmpeg",
    [
      "-y",
      '-i "' + copyFrom + '"',
      "-c:v libx264",
      "-preset slow",
      "-movflags faststart",
      "-profile:v high",
      "-level 4.2",
      "-max_muxing_queue_size 9999",
      '-vf "setpts=PTS/6,fps=30"',
      '-vsync vfr "' + copyTo + '"',
    ],
    {
      shell: true,
    }
  );

  ls.stderr.on("data", (data) => {
    console.error(`\tstderr: ${data}`);
  });

  console.log("---->>>> Finishing----------------------------------------");
  ls.on("close", (code) => {
    console.log(`\tConvertation process exited with code ${code}`);
    if (code == 0) {
      console.log("\tConvertion was done. Output patH: " + copyTo);
      console.log("---->>>> Cleanning----------------------------------------");
      fs.unlinkSync(copyFrom);
      console.log("\tCleaned Successfully " + copyFrom);
    }
    console.log("---->>>> Finished------------------------------------------");

    console.log("Bye");

    process.exit(0);
  });
}

function copyOnly(filename) {
  var copyFrom = homedir + "/Downloads/" + filename;
  var copyTo = copyToPath + "/" + filename;
  console.log("---->>>> Copying video file---------------------------------");
  console.log("\tCopy from: '" + copyFrom + "' to: '" + copyTo + "'");

  if (!fs.existsSync(copyToPath)) {
    fs.mkdirSync(copyToPath, { recursive: true });
  }

  try {
    fs.copyFileSync(copyFrom, copyTo);
    console.log("---->>>> Copied-------------------------------------------");
    console.log("\t" + copyTo);

    fs.unlinkSync(copyFrom);
    console.log("---->>>> Cleaned up---------------------------------------");
  } catch (err) {
    console.log(err);
  }
}

main();
