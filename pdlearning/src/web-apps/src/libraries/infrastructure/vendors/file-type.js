'use strict';

var _typeof =
  typeof Symbol === 'function' && typeof Symbol.iterator === 'symbol'
    ? function(obj) {
        return typeof obj;
      }
    : function(obj) {
        return obj && typeof Symbol === 'function' && obj.constructor === Symbol && obj !== Symbol.prototype ? 'symbol' : typeof obj;
      };

function _toConsumableArray(arr) {
  if (Array.isArray(arr)) {
    for (var i = 0, arr2 = Array(arr.length); i < arr.length; i++) {
      arr2[i] = arr[i];
    }
    return arr2;
  } else {
    return Array.from(arr);
  }
}

var supported = {
  extensions: [
    'jpg',
    'png',
    'apng',
    'gif',
    'webp',
    'flif',
    'cr2',
    'orf',
    'arw',
    'dng',
    'nef',
    'rw2',
    'raf',
    'tif',
    'bmp',
    'jxr',
    'psd',
    'zip',
    'tar',
    'rar',
    'gz',
    'bz2',
    '7z',
    'dmg',
    'mp4',
    'mid',
    'mkv',
    'webm',
    'mov',
    'avi',
    'mpg',
    'mp2',
    'mp3',
    'm4a',
    'oga',
    'ogg',
    'ogv',
    'opus',
    'flac',
    'wav',
    'spx',
    'amr',
    'pdf',
    'epub',
    'exe',
    'swf',
    'rtf',
    'wasm',
    'woff',
    'woff2',
    'eot',
    'ttf',
    'otf',
    'ico',
    'flv',
    'ps',
    'xz',
    'sqlite',
    'nes',
    'crx',
    'xpi',
    'cab',
    'deb',
    'ar',
    'rpm',
    'Z',
    'lz',
    'msi',
    'mxf',
    'mts',
    'blend',
    'bpg',
    'docx',
    'pptx',
    'xlsx',
    '3gp',
    '3g2',
    'jp2',
    'jpm',
    'jpx',
    'mj2',
    'aif',
    'qcp',
    'odt',
    'ods',
    'odp',
    'xml',
    'mobi',
    'heic',
    'cur',
    'ktx',
    'ape',
    'wv',
    'wmv',
    'wma',
    'dcm',
    'ics',
    'glb',
    'pcap',
    'dsf',
    'lnk',
    'alias',
    'voc',
    'ac3',
    'm4v',
    'm4p',
    'm4b',
    'f4v',
    'f4p',
    'f4b',
    'f4a',
    'mie',
    'asf',
    'ogm',
    'ogx',
    'mpc',
    'arrow',
    'shp'
  ],
  mimeTypes: [
    'image/jpeg',
    'image/png',
    'image/gif',
    'image/webp',
    'image/flif',
    'image/x-canon-cr2',
    'image/tiff',
    'image/bmp',
    'image/vnd.ms-photo',
    'image/vnd.adobe.photoshop',
    'application/epub+zip',
    'application/x-xpinstall',
    'application/vnd.oasis.opendocument.text',
    'application/vnd.oasis.opendocument.spreadsheet',
    'application/vnd.oasis.opendocument.presentation',
    'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
    'application/vnd.openxmlformats-officedocument.presentationml.presentation',
    'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet',
    'application/zip',
    'application/x-tar',
    'application/x-rar-compressed',
    'application/gzip',
    'application/x-bzip2',
    'application/x-7z-compressed',
    'application/x-apple-diskimage',
    'application/x-apache-arrow',
    'video/mp4',
    'audio/midi',
    'video/x-matroska',
    'video/webm',
    'video/quicktime',
    'video/vnd.avi',
    'audio/vnd.wave',
    'audio/qcelp',
    'audio/x-ms-wma',
    'video/x-ms-asf',
    'application/vnd.ms-asf',
    'video/mpeg',
    'video/3gpp',
    'audio/mpeg',
    'audio/mp4', // RFC 4337
    'audio/opus',
    'video/ogg',
    'audio/ogg',
    'application/ogg',
    'audio/x-flac',
    'audio/ape',
    'audio/wavpack',
    'audio/amr',
    'application/pdf',
    'application/x-msdownload',
    'application/x-shockwave-flash',
    'application/rtf',
    'application/wasm',
    'font/woff',
    'font/woff2',
    'application/vnd.ms-fontobject',
    'font/ttf',
    'font/otf',
    'image/x-icon',
    'video/x-flv',
    'application/postscript',
    'application/x-xz',
    'application/x-sqlite3',
    'application/x-nintendo-nes-rom',
    'application/x-google-chrome-extension',
    'application/vnd.ms-cab-compressed',
    'application/x-deb',
    'application/x-unix-archive',
    'application/x-rpm',
    'application/x-compress',
    'application/x-lzip',
    'application/x-msi',
    'application/x-mie',
    'application/mxf',
    'video/mp2t',
    'application/x-blender',
    'image/bpg',
    'image/jp2',
    'image/jpx',
    'image/jpm',
    'image/mj2',
    'audio/aiff',
    'application/xml',
    'application/x-mobipocket-ebook',
    'image/heif',
    'image/heif-sequence',
    'image/heic',
    'image/heic-sequence',
    'image/ktx',
    'application/dicom',
    'audio/x-musepack',
    'text/calendar',
    'model/gltf-binary',
    'application/vnd.tcpdump.pcap',
    'audio/x-dsf', // Non-standard
    'application/x.ms.shortcut', // Invented by us
    'application/x.apple.alias', // Invented by us
    'audio/x-voc',
    'audio/vnd.dolby.dd-raw',
    'audio/x-m4a',
    'image/apng',
    'image/x-olympus-orf',
    'image/x-sony-arw',
    'image/x-adobe-dng',
    'image/x-nikon-nef',
    'image/x-panasonic-rw2',
    'image/x-fujifilm-raf',
    'video/x-m4v',
    'video/3gpp2',
    'application/x-esri-shape'
  ]
};

var stringToBytes = function stringToBytes(string) {
  return [].concat(_toConsumableArray(string)).map(function(character) {
    return character.charCodeAt(0);
  });
};

var uint8ArrayUtf8ByteString = function uint8ArrayUtf8ByteString(array, start, end) {
  return String.fromCharCode.apply(String, _toConsumableArray(array.slice(start, end)));
};

var readUInt64LE = function readUInt64LE(buffer) {
  var offset = arguments.length > 1 && arguments[1] !== undefined ? arguments[1] : 0;

  var n = buffer[offset];
  var mul = 1;
  var i = 0;

  while (++i < 8) {
    mul *= 0x100;
    n += buffer[offset + i] * mul;
  }

  return n;
};

var tarHeaderChecksumMatches = function tarHeaderChecksumMatches(buffer) {
  // Does not check if checksum field characters are valid
  if (buffer.length < 512) {
    // `tar` header size, cannot compute checksum without it
    return false;
  }

  var MASK_8TH_BIT = 0x80;

  var sum = 256; // Intitalize sum, with 256 as sum of 8 spaces in checksum field
  var signedBitSum = 0; // Initialize signed bit sum

  for (var i = 0; i < 148; i++) {
    var byte = buffer[i];
    sum += byte;
    signedBitSum += byte & MASK_8TH_BIT; // Add signed bit to signed bit sum
  }

  // Skip checksum field

  for (var _i = 156; _i < 512; _i++) {
    var _byte = buffer[_i];
    sum += _byte;
    signedBitSum += _byte & MASK_8TH_BIT; // Add signed bit to signed bit sum
  }

  var readSum = parseInt(uint8ArrayUtf8ByteString(buffer, 148, 154), 8); // Read sum in header

  // Some implementations compute checksum incorrectly using signed bytes
  return (
    // Checksum in header equals the sum we calculated
    readSum === sum ||
    // Checksum in header equals sum we calculated plus signed-to-unsigned delta
    readSum === sum - (signedBitSum << 1)
  );
};

var multiByteIndexOf = function multiByteIndexOf(buffer, bytesToSearch) {
  var startAt = arguments.length > 2 && arguments[2] !== undefined ? arguments[2] : 0;

  // `Buffer#indexOf()` can search for multiple bytes
  if (Buffer && Buffer.isBuffer(buffer)) {
    return buffer.indexOf(Buffer.from(bytesToSearch), startAt);
  }

  var nextBytesMatch = function nextBytesMatch(buffer, bytes, startIndex) {
    for (var i = 1; i < bytes.length; i++) {
      if (bytes[i] !== buffer[startIndex + i]) {
        return false;
      }
    }

    return true;
  };

  // `Uint8Array#indexOf()` can search for only a single byte
  var index = buffer.indexOf(bytesToSearch[0], startAt);
  while (index >= 0) {
    if (nextBytesMatch(buffer, bytesToSearch, index)) {
      return index;
    }

    index = buffer.indexOf(bytesToSearch[0], index + 1);
  }

  return -1;
};

var xpiZipFilename = stringToBytes('META-INF/mozilla.rsa');
var oxmlContentTypes = stringToBytes('[Content_Types].xml');
var oxmlRels = stringToBytes('_rels/.rels');

var fileType = function fileType(input) {
  if (!(input instanceof Uint8Array || input instanceof ArrayBuffer || Buffer.isBuffer(input))) {
    throw new TypeError(
      'Expected the `input` argument to be of type `Uint8Array` or `Buffer` or `ArrayBuffer`, got `' +
        (typeof input === 'undefined' ? 'undefined' : _typeof(input)) +
        '`'
    );
  }

  var buffer = input instanceof Uint8Array ? input : new Uint8Array(input);

  if (!(buffer && buffer.length > 1)) {
    return;
  }

  var check = function check(header, options) {
    options = Object.assign({ offset: 0 }, options);

    for (var i = 0; i < header.length; i++) {
      // If a bitmask is set
      if (options.mask) {
        // If header doesn't equal `buf` with bits masked off
        if (header[i] !== (options.mask[i] & buffer[i + options.offset])) {
          return false;
        }
      } else if (header[i] !== buffer[i + options.offset]) {
        return false;
      }
    }

    return true;
  };

  var checkString = function checkString(header, options) {
    return check(stringToBytes(header), options);
  };

  if (check([0xff, 0xd8, 0xff])) {
    return {
      ext: 'jpg',
      mime: 'image/jpeg'
    };
  }

  if (check([0x89, 0x50, 0x4e, 0x47, 0x0d, 0x0a, 0x1a, 0x0a])) {
    // APNG format (https://wiki.mozilla.org/APNG_Specification)
    // 1. Find the first IDAT (image data) chunk (49 44 41 54)
    // 2. Check if there is an "acTL" chunk before the IDAT one (61 63 54 4C)

    // Offset calculated as follows:
    // - 8 bytes: PNG signature
    // - 4 (length) + 4 (chunk type) + 13 (chunk data) + 4 (CRC): IHDR chunk
    var startIndex = 33;
    var firstImageDataChunkIndex = buffer.findIndex(function(el, i) {
      return i >= startIndex && buffer[i] === 0x49 && buffer[i + 1] === 0x44 && buffer[i + 2] === 0x41 && buffer[i + 3] === 0x54;
    });
    var sliced = buffer.subarray(startIndex, firstImageDataChunkIndex);

    if (
      sliced.findIndex(function(el, i) {
        return sliced[i] === 0x61 && sliced[i + 1] === 0x63 && sliced[i + 2] === 0x54 && sliced[i + 3] === 0x4c;
      }) >= 0
    ) {
      return {
        ext: 'apng',
        mime: 'image/apng'
      };
    }

    return {
      ext: 'png',
      mime: 'image/png'
    };
  }

  if (check([0x47, 0x49, 0x46])) {
    return {
      ext: 'gif',
      mime: 'image/gif'
    };
  }

  if (check([0x57, 0x45, 0x42, 0x50], { offset: 8 })) {
    return {
      ext: 'webp',
      mime: 'image/webp'
    };
  }

  if (check([0x46, 0x4c, 0x49, 0x46])) {
    return {
      ext: 'flif',
      mime: 'image/flif'
    };
  }

  // `cr2`, `orf`, and `arw` need to be before `tif` check
  if ((check([0x49, 0x49, 0x2a, 0x0]) || check([0x4d, 0x4d, 0x0, 0x2a])) && check([0x43, 0x52], { offset: 8 })) {
    return {
      ext: 'cr2',
      mime: 'image/x-canon-cr2'
    };
  }

  if (check([0x49, 0x49, 0x52, 0x4f, 0x08, 0x00, 0x00, 0x00, 0x18])) {
    return {
      ext: 'orf',
      mime: 'image/x-olympus-orf'
    };
  }

  if (
    check([0x49, 0x49, 0x2a, 0x00]) &&
    (check([0x10, 0xfb, 0x86, 0x01], { offset: 4 }) || check([0x08, 0x00, 0x00, 0x00], { offset: 4 })) &&
    // This pattern differentiates ARW from other TIFF-ish file types:
    check([0x00, 0xfe, 0x00, 0x04, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x03, 0x01], { offset: 9 })
  ) {
    return {
      ext: 'arw',
      mime: 'image/x-sony-arw'
    };
  }

  if (
    check([0x49, 0x49, 0x2a, 0x00, 0x08, 0x00, 0x00, 0x00]) &&
    (check([0x2d, 0x00, 0xfe, 0x00], { offset: 8 }) || check([0x27, 0x00, 0xfe, 0x00], { offset: 8 }))
  ) {
    return {
      ext: 'dng',
      mime: 'image/x-adobe-dng'
    };
  }

  if (check([0x49, 0x49, 0x2a, 0x00]) && check([0x1c, 0x00, 0xfe, 0x00], { offset: 8 })) {
    return {
      ext: 'nef',
      mime: 'image/x-nikon-nef'
    };
  }

  if (check([0x49, 0x49, 0x55, 0x00, 0x18, 0x00, 0x00, 0x00, 0x88, 0xe7, 0x74, 0xd8])) {
    return {
      ext: 'rw2',
      mime: 'image/x-panasonic-rw2'
    };
  }

  // `raf` is here just to keep all the raw image detectors together.
  if (checkString('FUJIFILMCCD-RAW')) {
    return {
      ext: 'raf',
      mime: 'image/x-fujifilm-raf'
    };
  }

  if (check([0x49, 0x49, 0x2a, 0x0]) || check([0x4d, 0x4d, 0x0, 0x2a])) {
    return {
      ext: 'tif',
      mime: 'image/tiff'
    };
  }

  if (check([0x42, 0x4d])) {
    return {
      ext: 'bmp',
      mime: 'image/bmp'
    };
  }

  if (check([0x49, 0x49, 0xbc])) {
    return {
      ext: 'jxr',
      mime: 'image/vnd.ms-photo'
    };
  }

  if (check([0x38, 0x42, 0x50, 0x53])) {
    return {
      ext: 'psd',
      mime: 'image/vnd.adobe.photoshop'
    };
  }

  // Zip-based file formats
  // Need to be before the `zip` check
  var zipHeader = [0x50, 0x4b, 0x3, 0x4];
  if (check(zipHeader)) {
    if (
      check(
        [
          0x6d,
          0x69,
          0x6d,
          0x65,
          0x74,
          0x79,
          0x70,
          0x65,
          0x61,
          0x70,
          0x70,
          0x6c,
          0x69,
          0x63,
          0x61,
          0x74,
          0x69,
          0x6f,
          0x6e,
          0x2f,
          0x65,
          0x70,
          0x75,
          0x62,
          0x2b,
          0x7a,
          0x69,
          0x70
        ],
        { offset: 30 }
      )
    ) {
      return {
        ext: 'epub',
        mime: 'application/epub+zip'
      };
    }

    // Assumes signed `.xpi` from addons.mozilla.org
    if (check(xpiZipFilename, { offset: 30 })) {
      return {
        ext: 'xpi',
        mime: 'application/x-xpinstall'
      };
    }

    if (checkString('mimetypeapplication/vnd.oasis.opendocument.text', { offset: 30 })) {
      return {
        ext: 'odt',
        mime: 'application/vnd.oasis.opendocument.text'
      };
    }

    if (checkString('mimetypeapplication/vnd.oasis.opendocument.spreadsheet', { offset: 30 })) {
      return {
        ext: 'ods',
        mime: 'application/vnd.oasis.opendocument.spreadsheet'
      };
    }

    if (checkString('mimetypeapplication/vnd.oasis.opendocument.presentation', { offset: 30 })) {
      return {
        ext: 'odp',
        mime: 'application/vnd.oasis.opendocument.presentation'
      };
    }

    // The docx, xlsx and pptx file types extend the Office Open XML file format:
    // https://en.wikipedia.org/wiki/Office_Open_XML_file_formats
    // We look for:
    // - one entry named '[Content_Types].xml' or '_rels/.rels',
    // - one entry indicating specific type of file.
    // MS Office, OpenOffice and LibreOffice may put the parts in different order, so the check should not rely on it.
    var zipHeaderIndex = 0; // The first zip header was already found at index 0
    var oxmlFound = false;
    var type = void 0;

    do {
      var offset = zipHeaderIndex + 30;

      if (!oxmlFound) {
        oxmlFound = check(oxmlContentTypes, { offset: offset }) || check(oxmlRels, { offset: offset });
      }

      if (!type) {
        if (checkString('word/', { offset: offset })) {
          type = {
            ext: 'docx',
            mime: 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'
          };
        } else if (checkString('ppt/', { offset: offset })) {
          type = {
            ext: 'pptx',
            mime: 'application/vnd.openxmlformats-officedocument.presentationml.presentation'
          };
        } else if (checkString('xl/', { offset: offset })) {
          type = {
            ext: 'xlsx',
            mime: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet'
          };
        }
      }

      if (oxmlFound && type) {
        return type;
      }

      zipHeaderIndex = multiByteIndexOf(buffer, zipHeader, offset);
    } while (zipHeaderIndex >= 0);

    // No more zip parts available in the buffer, but maybe we are almost certain about the type?
    if (type) {
      return type;
    }
  }

  if (
    check([0x50, 0x4b]) &&
    (buffer[2] === 0x3 || buffer[2] === 0x5 || buffer[2] === 0x7) &&
    (buffer[3] === 0x4 || buffer[3] === 0x6 || buffer[3] === 0x8)
  ) {
    return {
      ext: 'zip',
      mime: 'application/zip'
    };
  }

  if (
    check([0x30, 0x30, 0x30, 0x30, 0x30, 0x30], { offset: 148, mask: [0xf8, 0xf8, 0xf8, 0xf8, 0xf8, 0xf8] }) && // Valid tar checksum
    tarHeaderChecksumMatches(buffer)
  ) {
    return {
      ext: 'tar',
      mime: 'application/x-tar'
    };
  }

  if (check([0x52, 0x61, 0x72, 0x21, 0x1a, 0x7]) && (buffer[6] === 0x0 || buffer[6] === 0x1)) {
    return {
      ext: 'rar',
      mime: 'application/x-rar-compressed'
    };
  }

  if (check([0x1f, 0x8b, 0x8])) {
    return {
      ext: 'gz',
      mime: 'application/gzip'
    };
  }

  if (check([0x42, 0x5a, 0x68])) {
    return {
      ext: 'bz2',
      mime: 'application/x-bzip2'
    };
  }

  if (check([0x37, 0x7a, 0xbc, 0xaf, 0x27, 0x1c])) {
    return {
      ext: '7z',
      mime: 'application/x-7z-compressed'
    };
  }

  if (check([0x78, 0x01])) {
    return {
      ext: 'dmg',
      mime: 'application/x-apple-diskimage'
    };
  }

  // `mov` format variants
  if (
    check([0x66, 0x72, 0x65, 0x65], { offset: 4 }) || // `free`
    check([0x6d, 0x64, 0x61, 0x74], { offset: 4 }) || // `mdat` MJPEG
    check([0x6d, 0x6f, 0x6f, 0x76], { offset: 4 }) || // `moov`
    check([0x77, 0x69, 0x64, 0x65], { offset: 4 }) // `wide`
  ) {
    return {
      ext: 'mov',
      mime: 'video/quicktime'
    };
  }

  // File Type Box (https://en.wikipedia.org/wiki/ISO_base_media_file_format)
  // It's not required to be first, but it's recommended to be. Almost all ISO base media files start with `ftyp` box.
  // `ftyp` box must contain a brand major identifier, which must consist of ISO 8859-1 printable characters.
  // Here we check for 8859-1 printable characters (for simplicity, it's a mask which also catches one non-printable character).
  if (
    checkString('ftyp', { offset: 4 }) &&
    (buffer[8] & 0x60) !== 0x00 // Brand major, first character ASCII?
  ) {
    // They all can have MIME `video/mp4` except `application/mp4` special-case which is hard to detect.
    // For some cases, we're specific, everything else falls to `video/mp4` with `mp4` extension.
    var brandMajor = uint8ArrayUtf8ByteString(buffer, 8, 12)
      .replace('\0', ' ')
      .trim();
    switch (brandMajor) {
      case 'mif1':
        return { ext: 'heic', mime: 'image/heif' };
      case 'msf1':
        return { ext: 'heic', mime: 'image/heif-sequence' };
      case 'heic':
      case 'heix':
        return { ext: 'heic', mime: 'image/heic' };
      case 'hevc':
      case 'hevx':
        return { ext: 'heic', mime: 'image/heic-sequence' };
      case 'qt':
        return { ext: 'mov', mime: 'video/quicktime' };
      case 'M4V':
      case 'M4VH':
      case 'M4VP':
        return { ext: 'm4v', mime: 'video/x-m4v' };
      case 'M4P':
        return { ext: 'm4p', mime: 'video/mp4' };
      case 'M4B':
        return { ext: 'm4b', mime: 'audio/mp4' };
      case 'M4A':
        return { ext: 'm4a', mime: 'audio/x-m4a' };
      case 'F4V':
        return { ext: 'f4v', mime: 'video/mp4' };
      case 'F4P':
        return { ext: 'f4p', mime: 'video/mp4' };
      case 'F4A':
        return { ext: 'f4a', mime: 'audio/mp4' };
      case 'F4B':
        return { ext: 'f4b', mime: 'audio/mp4' };
      default:
        if (brandMajor.startsWith('3g')) {
          if (brandMajor.startsWith('3g2')) {
            return { ext: '3g2', mime: 'video/3gpp2' };
          }

          return { ext: '3gp', mime: 'video/3gpp' };
        }

        return { ext: 'mp4', mime: 'video/mp4' };
    }
  }

  if (check([0x4d, 0x54, 0x68, 0x64])) {
    return {
      ext: 'mid',
      mime: 'audio/midi'
    };
  }

  // https://github.com/threatstack/libmagic/blob/master/magic/Magdir/matroska
  if (check([0x1a, 0x45, 0xdf, 0xa3])) {
    var _sliced = buffer.subarray(4, 4 + 4096);
    var idPos = _sliced.findIndex(function(el, i, arr) {
      return arr[i] === 0x42 && arr[i + 1] === 0x82;
    });

    if (idPos !== -1) {
      var docTypePos = idPos + 3;
      var findDocType = function findDocType(type) {
        return [].concat(_toConsumableArray(type)).every(function(c, i) {
          return _sliced[docTypePos + i] === c.charCodeAt(0);
        });
      };

      if (findDocType('matroska')) {
        return {
          ext: 'mkv',
          mime: 'video/x-matroska'
        };
      }

      if (findDocType('webm')) {
        return {
          ext: 'webm',
          mime: 'video/webm'
        };
      }
    }
  }

  // RIFF file format which might be AVI, WAV, QCP, etc
  if (check([0x52, 0x49, 0x46, 0x46])) {
    if (check([0x41, 0x56, 0x49], { offset: 8 })) {
      return {
        ext: 'avi',
        mime: 'video/vnd.avi'
      };
    }

    if (check([0x57, 0x41, 0x56, 0x45], { offset: 8 })) {
      return {
        ext: 'wav',
        mime: 'audio/vnd.wave'
      };
    }

    // QLCM, QCP file
    if (check([0x51, 0x4c, 0x43, 0x4d], { offset: 8 })) {
      return {
        ext: 'qcp',
        mime: 'audio/qcelp'
      };
    }
  }

  // ASF_Header_Object first 80 bytes
  if (check([0x30, 0x26, 0xb2, 0x75, 0x8e, 0x66, 0xcf, 0x11, 0xa6, 0xd9])) {
    // Search for header should be in first 1KB of file.

    var _offset = 30;
    do {
      var objectSize = readUInt64LE(buffer, _offset + 16);
      if (check([0x91, 0x07, 0xdc, 0xb7, 0xb7, 0xa9, 0xcf, 0x11, 0x8e, 0xe6, 0x00, 0xc0, 0x0c, 0x20, 0x53, 0x65], { offset: _offset })) {
        // Sync on Stream-Properties-Object (B7DC0791-A9B7-11CF-8EE6-00C00C205365)
        if (
          check([0x40, 0x9e, 0x69, 0xf8, 0x4d, 0x5b, 0xcf, 0x11, 0xa8, 0xfd, 0x00, 0x80, 0x5f, 0x5c, 0x44, 0x2b], { offset: _offset + 24 })
        ) {
          // Found audio:
          return {
            ext: 'wma',
            mime: 'audio/x-ms-wma'
          };
        }

        if (
          check([0xc0, 0xef, 0x19, 0xbc, 0x4d, 0x5b, 0xcf, 0x11, 0xa8, 0xfd, 0x00, 0x80, 0x5f, 0x5c, 0x44, 0x2b], { offset: _offset + 24 })
        ) {
          // Found video:
          return {
            ext: 'wmv',
            mime: 'video/x-ms-asf'
          };
        }

        break;
      }

      _offset += objectSize;
    } while (_offset + 24 <= buffer.length);

    // Default to ASF generic extension
    return {
      ext: 'asf',
      mime: 'application/vnd.ms-asf'
    };
  }

  if (check([0x0, 0x0, 0x1, 0xba]) || check([0x0, 0x0, 0x1, 0xb3])) {
    return {
      ext: 'mpg',
      mime: 'video/mpeg'
    };
  }

  // Check for MPEG header at different starting offsets
  for (var start = 0; start < 2 && start < buffer.length - 16; start++) {
    if (
      check([0x49, 0x44, 0x33], { offset: start }) || // ID3 header
      check([0xff, 0xe2], { offset: start, mask: [0xff, 0xe6] }) // MPEG 1 or 2 Layer 3 header
    ) {
      return {
        ext: 'mp3',
        mime: 'audio/mpeg'
      };
    }

    if (
      check([0xff, 0xe4], { offset: start, mask: [0xff, 0xe6] }) // MPEG 1 or 2 Layer 2 header
    ) {
      return {
        ext: 'mp2',
        mime: 'audio/mpeg'
      };
    }

    if (
      check([0xff, 0xf8], { offset: start, mask: [0xff, 0xfc] }) // MPEG 2 layer 0 using ADTS
    ) {
      return {
        ext: 'mp2',
        mime: 'audio/mpeg'
      };
    }

    if (
      check([0xff, 0xf0], { offset: start, mask: [0xff, 0xfc] }) // MPEG 4 layer 0 using ADTS
    ) {
      return {
        ext: 'mp4',
        mime: 'audio/mpeg'
      };
    }
  }

  // Needs to be before `ogg` check
  if (check([0x4f, 0x70, 0x75, 0x73, 0x48, 0x65, 0x61, 0x64], { offset: 28 })) {
    return {
      ext: 'opus',
      mime: 'audio/opus'
    };
  }

  // If 'OggS' in first  bytes, then OGG container
  if (check([0x4f, 0x67, 0x67, 0x53])) {
    // This is a OGG container

    // If ' theora' in header.
    if (check([0x80, 0x74, 0x68, 0x65, 0x6f, 0x72, 0x61], { offset: 28 })) {
      return {
        ext: 'ogv',
        mime: 'video/ogg'
      };
    }

    // If '\x01video' in header.
    if (check([0x01, 0x76, 0x69, 0x64, 0x65, 0x6f, 0x00], { offset: 28 })) {
      return {
        ext: 'ogm',
        mime: 'video/ogg'
      };
    }

    // If ' FLAC' in header  https://xiph.org/flac/faq.html
    if (check([0x7f, 0x46, 0x4c, 0x41, 0x43], { offset: 28 })) {
      return {
        ext: 'oga',
        mime: 'audio/ogg'
      };
    }

    // 'Speex  ' in header https://en.wikipedia.org/wiki/Speex
    if (check([0x53, 0x70, 0x65, 0x65, 0x78, 0x20, 0x20], { offset: 28 })) {
      return {
        ext: 'spx',
        mime: 'audio/ogg'
      };
    }

    // If '\x01vorbis' in header
    if (check([0x01, 0x76, 0x6f, 0x72, 0x62, 0x69, 0x73], { offset: 28 })) {
      return {
        ext: 'ogg',
        mime: 'audio/ogg'
      };
    }

    // Default OGG container https://www.iana.org/assignments/media-types/application/ogg
    return {
      ext: 'ogx',
      mime: 'application/ogg'
    };
  }

  if (check([0x66, 0x4c, 0x61, 0x43])) {
    return {
      ext: 'flac',
      mime: 'audio/x-flac'
    };
  }

  if (check([0x4d, 0x41, 0x43, 0x20])) {
    // 'MAC '
    return {
      ext: 'ape',
      mime: 'audio/ape'
    };
  }

  if (check([0x77, 0x76, 0x70, 0x6b])) {
    // 'wvpk'
    return {
      ext: 'wv',
      mime: 'audio/wavpack'
    };
  }

  if (check([0x23, 0x21, 0x41, 0x4d, 0x52, 0x0a])) {
    return {
      ext: 'amr',
      mime: 'audio/amr'
    };
  }

  if (check([0x25, 0x50, 0x44, 0x46])) {
    return {
      ext: 'pdf',
      mime: 'application/pdf'
    };
  }

  if (check([0x4d, 0x5a])) {
    return {
      ext: 'exe',
      mime: 'application/x-msdownload'
    };
  }

  if ((buffer[0] === 0x43 || buffer[0] === 0x46) && check([0x57, 0x53], { offset: 1 })) {
    return {
      ext: 'swf',
      mime: 'application/x-shockwave-flash'
    };
  }

  if (check([0x7b, 0x5c, 0x72, 0x74, 0x66])) {
    return {
      ext: 'rtf',
      mime: 'application/rtf'
    };
  }

  if (check([0x00, 0x61, 0x73, 0x6d])) {
    return {
      ext: 'wasm',
      mime: 'application/wasm'
    };
  }

  if (
    check([0x77, 0x4f, 0x46, 0x46]) &&
    (check([0x00, 0x01, 0x00, 0x00], { offset: 4 }) || check([0x4f, 0x54, 0x54, 0x4f], { offset: 4 }))
  ) {
    return {
      ext: 'woff',
      mime: 'font/woff'
    };
  }

  if (
    check([0x77, 0x4f, 0x46, 0x32]) &&
    (check([0x00, 0x01, 0x00, 0x00], { offset: 4 }) || check([0x4f, 0x54, 0x54, 0x4f], { offset: 4 }))
  ) {
    return {
      ext: 'woff2',
      mime: 'font/woff2'
    };
  }

  if (
    check([0x4c, 0x50], { offset: 34 }) &&
    (check([0x00, 0x00, 0x01], { offset: 8 }) || check([0x01, 0x00, 0x02], { offset: 8 }) || check([0x02, 0x00, 0x02], { offset: 8 }))
  ) {
    return {
      ext: 'eot',
      mime: 'application/vnd.ms-fontobject'
    };
  }

  if (check([0x00, 0x01, 0x00, 0x00, 0x00])) {
    return {
      ext: 'ttf',
      mime: 'font/ttf'
    };
  }

  if (check([0x4f, 0x54, 0x54, 0x4f, 0x00])) {
    return {
      ext: 'otf',
      mime: 'font/otf'
    };
  }

  if (check([0x00, 0x00, 0x01, 0x00])) {
    return {
      ext: 'ico',
      mime: 'image/x-icon'
    };
  }

  if (check([0x00, 0x00, 0x02, 0x00])) {
    return {
      ext: 'cur',
      mime: 'image/x-icon'
    };
  }

  if (check([0x46, 0x4c, 0x56, 0x01])) {
    return {
      ext: 'flv',
      mime: 'video/x-flv'
    };
  }

  if (check([0x25, 0x21])) {
    return {
      ext: 'ps',
      mime: 'application/postscript'
    };
  }

  if (check([0xfd, 0x37, 0x7a, 0x58, 0x5a, 0x00])) {
    return {
      ext: 'xz',
      mime: 'application/x-xz'
    };
  }

  if (check([0x53, 0x51, 0x4c, 0x69])) {
    return {
      ext: 'sqlite',
      mime: 'application/x-sqlite3'
    };
  }

  if (check([0x4e, 0x45, 0x53, 0x1a])) {
    return {
      ext: 'nes',
      mime: 'application/x-nintendo-nes-rom'
    };
  }

  if (check([0x43, 0x72, 0x32, 0x34])) {
    return {
      ext: 'crx',
      mime: 'application/x-google-chrome-extension'
    };
  }

  if (check([0x4d, 0x53, 0x43, 0x46]) || check([0x49, 0x53, 0x63, 0x28])) {
    return {
      ext: 'cab',
      mime: 'application/vnd.ms-cab-compressed'
    };
  }

  // Needs to be before `ar` check
  if (
    check([0x21, 0x3c, 0x61, 0x72, 0x63, 0x68, 0x3e, 0x0a, 0x64, 0x65, 0x62, 0x69, 0x61, 0x6e, 0x2d, 0x62, 0x69, 0x6e, 0x61, 0x72, 0x79])
  ) {
    return {
      ext: 'deb',
      mime: 'application/x-deb'
    };
  }

  if (check([0x21, 0x3c, 0x61, 0x72, 0x63, 0x68, 0x3e])) {
    return {
      ext: 'ar',
      mime: 'application/x-unix-archive'
    };
  }

  if (check([0xed, 0xab, 0xee, 0xdb])) {
    return {
      ext: 'rpm',
      mime: 'application/x-rpm'
    };
  }

  if (check([0x1f, 0xa0]) || check([0x1f, 0x9d])) {
    return {
      ext: 'Z',
      mime: 'application/x-compress'
    };
  }

  if (check([0x4c, 0x5a, 0x49, 0x50])) {
    return {
      ext: 'lz',
      mime: 'application/x-lzip'
    };
  }

  if (
    check([
      0xd0,
      0xcf,
      0x11,
      0xe0,
      0xa1,
      0xb1,
      0x1a,
      0xe1,
      0x00,
      0x00,
      0x00,
      0x00,
      0x00,
      0x00,
      0x00,
      0x00,
      0x00,
      0x00,
      0x00,
      0x00,
      0x00,
      0x00,
      0x00,
      0x00,
      0x3e
    ])
  ) {
    return {
      ext: 'msi',
      mime: 'application/x-msi'
    };
  }

  if (check([0x06, 0x0e, 0x2b, 0x34, 0x02, 0x05, 0x01, 0x01, 0x0d, 0x01, 0x02, 0x01, 0x01, 0x02])) {
    return {
      ext: 'mxf',
      mime: 'application/mxf'
    };
  }

  if (check([0x47], { offset: 4 }) && (check([0x47], { offset: 192 }) || check([0x47], { offset: 196 }))) {
    return {
      ext: 'mts',
      mime: 'video/mp2t'
    };
  }

  if (check([0x42, 0x4c, 0x45, 0x4e, 0x44, 0x45, 0x52])) {
    return {
      ext: 'blend',
      mime: 'application/x-blender'
    };
  }

  if (check([0x42, 0x50, 0x47, 0xfb])) {
    return {
      ext: 'bpg',
      mime: 'image/bpg'
    };
  }

  if (check([0x00, 0x00, 0x00, 0x0c, 0x6a, 0x50, 0x20, 0x20, 0x0d, 0x0a, 0x87, 0x0a])) {
    // JPEG-2000 family

    if (check([0x6a, 0x70, 0x32, 0x20], { offset: 20 })) {
      return {
        ext: 'jp2',
        mime: 'image/jp2'
      };
    }

    if (check([0x6a, 0x70, 0x78, 0x20], { offset: 20 })) {
      return {
        ext: 'jpx',
        mime: 'image/jpx'
      };
    }

    if (check([0x6a, 0x70, 0x6d, 0x20], { offset: 20 })) {
      return {
        ext: 'jpm',
        mime: 'image/jpm'
      };
    }

    if (check([0x6d, 0x6a, 0x70, 0x32], { offset: 20 })) {
      return {
        ext: 'mj2',
        mime: 'image/mj2'
      };
    }
  }

  if (check([0x46, 0x4f, 0x52, 0x4d])) {
    return {
      ext: 'aif',
      mime: 'audio/aiff'
    };
  }

  if (checkString('<?xml ')) {
    return {
      ext: 'xml',
      mime: 'application/xml'
    };
  }

  if (check([0x42, 0x4f, 0x4f, 0x4b, 0x4d, 0x4f, 0x42, 0x49], { offset: 60 })) {
    return {
      ext: 'mobi',
      mime: 'application/x-mobipocket-ebook'
    };
  }

  if (check([0xab, 0x4b, 0x54, 0x58, 0x20, 0x31, 0x31, 0xbb, 0x0d, 0x0a, 0x1a, 0x0a])) {
    return {
      ext: 'ktx',
      mime: 'image/ktx'
    };
  }

  if (check([0x44, 0x49, 0x43, 0x4d], { offset: 128 })) {
    return {
      ext: 'dcm',
      mime: 'application/dicom'
    };
  }

  // Musepack, SV7
  if (check([0x4d, 0x50, 0x2b])) {
    return {
      ext: 'mpc',
      mime: 'audio/x-musepack'
    };
  }

  // Musepack, SV8
  if (check([0x4d, 0x50, 0x43, 0x4b])) {
    return {
      ext: 'mpc',
      mime: 'audio/x-musepack'
    };
  }

  if (check([0x42, 0x45, 0x47, 0x49, 0x4e, 0x3a])) {
    return {
      ext: 'ics',
      mime: 'text/calendar'
    };
  }

  if (check([0x67, 0x6c, 0x54, 0x46, 0x02, 0x00, 0x00, 0x00])) {
    return {
      ext: 'glb',
      mime: 'model/gltf-binary'
    };
  }

  if (check([0xd4, 0xc3, 0xb2, 0xa1]) || check([0xa1, 0xb2, 0xc3, 0xd4])) {
    return {
      ext: 'pcap',
      mime: 'application/vnd.tcpdump.pcap'
    };
  }

  // Sony DSD Stream File (DSF)
  if (check([0x44, 0x53, 0x44, 0x20])) {
    return {
      ext: 'dsf',
      mime: 'audio/x-dsf' // Non-standard
    };
  }

  if (check([0x4c, 0x00, 0x00, 0x00, 0x01, 0x14, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0xc0, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x46])) {
    return {
      ext: 'lnk',
      mime: 'application/x.ms.shortcut' // Invented by us
    };
  }

  if (check([0x62, 0x6f, 0x6f, 0x6b, 0x00, 0x00, 0x00, 0x00, 0x6d, 0x61, 0x72, 0x6b, 0x00, 0x00, 0x00, 0x00])) {
    return {
      ext: 'alias',
      mime: 'application/x.apple.alias' // Invented by us
    };
  }

  if (checkString('Creative Voice File')) {
    return {
      ext: 'voc',
      mime: 'audio/x-voc'
    };
  }

  if (check([0x0b, 0x77])) {
    return {
      ext: 'ac3',
      mime: 'audio/vnd.dolby.dd-raw'
    };
  }

  if ((check([0x7e, 0x10, 0x04]) || check([0x7e, 0x18, 0x04])) && check([0x30, 0x4d, 0x49, 0x45], { offset: 4 })) {
    return {
      ext: 'mie',
      mime: 'application/x-mie'
    };
  }

  if (check([0x41, 0x52, 0x52, 0x4f, 0x57, 0x31, 0x00, 0x00])) {
    return {
      ext: 'arrow',
      mime: 'application/x-apache-arrow'
    };
  }

  if (check([0x27, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00], { offset: 2 })) {
    return {
      ext: 'shp',
      mime: 'application/x-esri-shape'
    };
  }
};

module.exports = fileType;

Object.defineProperty(fileType, 'minimumBytes', { value: 4100 });

fileType.stream = function(readableStream) {
  return new Promise(function(resolve, reject) {
    // Using `eval` to work around issues when bundling with Webpack
    var stream = eval('require')('stream'); // eslint-disable-line no-eval

    readableStream.on('error', reject);
    readableStream.once('readable', function() {
      var pass = new stream.PassThrough();
      var chunk = readableStream.read(module.exports.minimumBytes) || readableStream.read();
      try {
        pass.fileType = fileType(chunk);
      } catch (error) {
        reject(error);
        return;
      }

      readableStream.unshift(chunk);

      if (stream.pipeline) {
        resolve(stream.pipeline(readableStream, pass, function() {}));
      } else {
        resolve(readableStream.pipe(pass));
      }
    });
  });
};

Object.defineProperty(fileType, 'extensions', {
  get: function get() {
    return new Set(supported.extensions);
  }
});

Object.defineProperty(fileType, 'mimeTypes', {
  get: function get() {
    return new Set(supported.mimeTypes);
  }
});
