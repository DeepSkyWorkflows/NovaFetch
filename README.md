# Nova Fetch

Nova Fetch is a commandline tool for plate solving images and retrieving the results. It is a tool I built specifically for the [Deep Sky Workflows Gallery](https://deepskyworkflows.com/gallery/),
but I'm sharing the source code in case it is helpful for others to build similar tools. If you're not aware, [Nova Astrometry](https://nova.astrometry.net) has excellent algorithms to plate 
solve or identify the dimensions and objects in your astrophotographs. When you upload a job, it solves the dimensions, generates metadata like the coordinates and dimensions of your image,
and creates annotated versions with labels and gridlines. I use the output of this for my gallery. 

## Usage

The first step is to obtain an API key. [Sign in](https://nova.astrometry.net/signin/) to create your account, then go to [this page](https://nova.astrometry.net/api_help) to obtain your
API key. The tool will look for the key in an environment variable named `novatoken`.

Next, pass three arguments to the `NovaFetch` tool:

1. A short name for the object of interest, i.e. M31 or "Andromeda" for the Andromeda galaxy. This short name will be used to name the files that 
are retrieved.
1. The path to your image that you wish to process.
1. The path to the target folder.

For example, assume I have an image of the Andromeda galaxy and I want to plate-solve it and generate the images. They should be stored in a folder I named "Andromeda."

I type:

`NovaFetch.exe m31 /d/astro/final/m31-andromeda/m31_208_scaled.jpg /c/temp/andromeda`

The output looks like this:

```text
NovaFetch v0.1.0.0
Using target filename: m31
Established session with id 0nmzd1f1mhirsffpyr2fpwkg0swsuqh6.
Target file: m31_208_scaled.jpg
Target directory: c:/temp/andromeda
Uploading d:/astro/final/m31-andromeda/m31_208_scaled.jpg to Nova...
Success! Submission id is 4868374
Getting status for job 4868374...
Processing started...
..........Downloading result files...
Downloading m31-annotated.jpg...
Downloading m31-grid.jpg...
Downloading m31-annotated-fs.jpg...
---
title: "m31"
type:
tags: ["NGC 224","M 31","Andromeda Galaxy"]
description:
image: /assets/images/gallery/m31/thumb.jpg
telescope: Stellina
length: "400mm"
aperture: "80mm"
folder: c:/temp/andromeda
exposure:
lights:
sessions:
firstCapture:
lastCapture:
ra: "0h 42m 40.6s"
dec: "+41° 15' 33.092"
size: "61.005 x 41.187 arcmin"
radius: "0.613 deg"
scale: "0.989 arcsec/pixel"
---
Writing data to c:/temp/andromeda\m31.md
Final tasks...
Copying original file...
Creating thumbnail...
Done.
```

This indicates a successful job. When it is done, I open the target folder and it contains:

- `__m31.jpg__` is your original file, renamed and copied. 
- `__m31.md__` is a facts page with stats taken from the server like right ascension and declination. I move this to a special folder to register a new gallery item.
- `__m31-annotated.jpg__` is a smaller version of the source file updated with the annotations.
- `__m31-annotated-fs.jpg is a full-sized version of annotations.
- `__m31_grid__` is your image with a gid overlay.
- `__thumb.jpg__` is a scaled down version of the image.

## The `-e` switch

If you have an existing job, or if a previous one fails, you can run the command with the `-e` switch to avoid resubmitting. Just pass the job number as the next parameter and it will pick up
where you left off. For example, assume my Andromeda job aborted halfway through. I can run:

`NovaFetch.exe -e 4868374 m31 /d/astro/final/m31-andromeda/m31_208_scaled.jpg /c/temp/andromeda`

This will overwrite existing files. 

## The `-s` switch

In case you just want to submit it and aren't interested in downloading result, pass the `-s` switch for _submit only_. You can always retrieve it later using
the `-e` switch.

