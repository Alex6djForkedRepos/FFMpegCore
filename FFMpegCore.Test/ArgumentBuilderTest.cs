﻿using System.Drawing;
using FFMpegCore.Arguments;
using FFMpegCore.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FFMpegCore.Test
{
    [TestClass]
    public class ArgumentBuilderTest
    {
        private readonly string[] _concatFiles = { "1.mp4", "2.mp4", "3.mp4", "4.mp4" };
        private readonly string[] _multiFiles = { "1.mp3", "2.mp3", "3.mp3", "4.mp3" };

        [TestMethod]
        public void Builder_BuildString_IO_1()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4").OutputToFile("output.mp4").Arguments;
            Assert.AreEqual("-i \"input.mp4\" \"output.mp4\" -y", str);
        }

        [TestMethod]
        public void Builder_BuildString_Scale()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", true, opt => opt
                    .WithVideoFilters(filterOptions => filterOptions
                        .Scale(VideoSize.Hd)))
                .Arguments;
            Assert.AreEqual("-i \"input.mp4\" -vf \"scale=-1:720\" \"output.mp4\" -y", str);
        }

        [TestMethod]
        public void Builder_BuildString_AudioCodec()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", true, opt => opt.WithAudioCodec(AudioCodec.Aac)).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -c:a aac \"output.mp4\" -y", str);
        }

        [TestMethod]
        public void Builder_BuildString_AudioBitrate()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", true, opt => opt.WithAudioBitrate(AudioQuality.Normal)).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -b:a 128k \"output.mp4\" -y", str);
        }

        [TestMethod]
        public void Builder_BuildString_Quiet()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4").WithGlobalOptions(opt => opt.WithVerbosityLevel())
                .OutputToFile("output.mp4", false).Arguments;
            Assert.AreEqual("-hide_banner -loglevel error -i \"input.mp4\" \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_AudioCodec_Fluent()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4").OutputToFile("output.mp4", false,
                opt => opt.WithAudioCodec(AudioCodec.Aac).WithAudioBitrate(128)).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -c:a aac -b:a 128k \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_BitStream()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4").OutputToFile("output.mp4", false,
                opt => opt.WithBitStreamFilter(Channel.Audio, Filter.H264_Mp4ToAnnexB)).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -bsf:a h264_mp4toannexb \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_HardwareAcceleration_Auto()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt.WithHardwareAcceleration()).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -hwaccel auto \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_HardwareAcceleration_Specific()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4").OutputToFile("output.mp4", false,
                opt => opt.WithHardwareAcceleration(HardwareAccelerationDevice.CUVID)).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -hwaccel cuvid \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_Concat()
        {
            var str = FFMpegArguments.FromConcatInput(_concatFiles).OutputToFile("output.mp4", false).Arguments;
            Assert.AreEqual("-i \"concat:1.mp4|2.mp4|3.mp4|4.mp4\" \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_Copy_Audio()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt.CopyChannel(Channel.Audio)).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -c:a copy \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_Copy_Video()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt.CopyChannel(Channel.Video)).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -c:v copy \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_Copy_Both()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt.CopyChannel()).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -c:a copy -c:v copy \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_DisableChannel_Audio()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt.DisableChannel(Channel.Audio)).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -an \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_DisableChannel_Video()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt.DisableChannel(Channel.Video)).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -vn \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_AudioSamplingRate_Default()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt.WithAudioSamplingRate()).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -ar 48000 \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_AudioSamplingRate()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt.WithAudioSamplingRate(44000)).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -ar 44000 \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_VariableBitrate()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt.WithVariableBitrate(5)).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -vbr 5 \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_Faststart()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt.WithFastStart()).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -movflags faststart \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_Overwrite()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt.OverwriteExisting()).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -y \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_RemoveMetadata()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt.WithoutMetadata()).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -map_metadata -1 \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_Transpose()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt
                    .WithVideoFilters(filterOptions => filterOptions
                        .Transpose(Transposition.CounterClockwise90)))
                .Arguments;
            Assert.AreEqual("-i \"input.mp4\" -vf \"transpose=2\" \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_Mirroring()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt
                    .WithVideoFilters(filterOptions => filterOptions
                        .Mirror(Mirroring.Horizontal)))
                .Arguments;
            Assert.AreEqual("-i \"input.mp4\" -vf \"hflip\" \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_TransposeScale()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt
                    .WithVideoFilters(filterOptions => filterOptions
                        .Transpose(Transposition.CounterClockwise90)
                        .Scale(200, 300)))
                .Arguments;
            Assert.AreEqual("-i \"input.mp4\" -vf \"transpose=2, scale=200:300\" \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_ForceFormat()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4", false, opt => opt.ForceFormat(VideoType.Mp4))
                .OutputToFile("output.mp4", false, opt => opt.ForceFormat(VideoType.Mp4)).Arguments;
            Assert.AreEqual("-f mp4 -i \"input.mp4\" -f mp4 \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_FrameOutputCount()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt.WithFrameOutputCount(50)).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -vframes 50 \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_VideoStreamNumber()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4").OutputToFile("output.mp4", false, opt => opt.SelectStream(1)).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -map 0:1 \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_FrameRate()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt.WithFramerate(50)).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -r 50 \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_Loop()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4").OutputToFile("output.mp4", false, opt => opt.Loop(50))
                .Arguments;
            Assert.AreEqual("-i \"input.mp4\" -loop 50 \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_Seek()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4", false, opt => opt.Seek(TimeSpan.FromSeconds(10))).OutputToFile("output.mp4", false, opt => opt.Seek(TimeSpan.FromSeconds(10))).Arguments;
            Assert.AreEqual("-ss 00:00:10.000 -i \"input.mp4\" -ss 00:00:10.000 \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_EndSeek()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4", false, opt => opt.EndSeek(TimeSpan.FromSeconds(10))).OutputToFile("output.mp4", false, opt => opt.EndSeek(TimeSpan.FromSeconds(10))).Arguments;
            Assert.AreEqual("-to 00:00:10.000 -i \"input.mp4\" -to 00:00:10.000 \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_Shortest()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt.UsingShortest()).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -shortest \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_Size()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt.Resize(1920, 1080)).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -s 1920x1080 \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_Speed()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt.WithSpeedPreset(Speed.Fast)).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -preset fast \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_DrawtextFilter()
        {
            var str = FFMpegArguments
                .FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt
                    .WithVideoFilters(filterOptions => filterOptions
                        .DrawText(DrawTextOptions
                            .Create("Stack Overflow", "/path/to/font.ttf")
                            .WithParameter("fontcolor", "white")
                            .WithParameter("fontsize", "24")
                            .WithParameter("box", "1")
                            .WithParameter("boxcolor", "black@0.5")
                            .WithParameter("boxborderw", "5")
                            .WithParameter("x", "(w-text_w)/2")
                            .WithParameter("y", "(h-text_h)/2"))))
                .Arguments;

            Assert.AreEqual(
                "-i \"input.mp4\" -vf \"drawtext=text='Stack Overflow':fontfile=/path/to/font.ttf:fontcolor=white:fontsize=24:box=1:boxcolor=black@0.5:boxborderw=5:x=(w-text_w)/2:y=(h-text_h)/2\" \"output.mp4\"",
                str);
        }

        [TestMethod]
        public void Builder_BuildString_DrawtextFilter_Alt()
        {
            var str = FFMpegArguments
                .FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt
                    .WithVideoFilters(filterOptions => filterOptions
                        .DrawText(DrawTextOptions
                            .Create("Stack Overflow", "/path/to/font.ttf", ("fontcolor", "white"), ("fontsize", "24")))))
                .Arguments;

            Assert.AreEqual(
                "-i \"input.mp4\" -vf \"drawtext=text='Stack Overflow':fontfile=/path/to/font.ttf:fontcolor=white:fontsize=24\" \"output.mp4\"",
                str);
        }

        [TestMethod]
        public void Builder_BuildString_SubtitleHardBurnFilter()
        {
            var str = FFMpegArguments
                .FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt
                    .WithVideoFilters(filterOptions => filterOptions
                        .HardBurnSubtitle(SubtitleHardBurnOptions
                            .Create(subtitlePath: "sample.srt")
                            .SetCharacterEncoding("UTF-8")
                            .SetOriginalSize(1366, 768)
                            .SetSubtitleIndex(0)
                            .WithStyle(StyleOptions.Create()
                                .WithParameter("FontName", "DejaVu Serif")
                                .WithParameter("PrimaryColour", "&HAA00FF00")))))
                .Arguments;

            Assert.AreEqual("-i \"input.mp4\" -vf \"subtitles='sample.srt':charenc=UTF-8:original_size=1366x768:stream_index=0:force_style='FontName=DejaVu Serif\\,PrimaryColour=&HAA00FF00'\" \"output.mp4\"",
                str);
        }

        [TestMethod]
        public void Builder_BuildString_SubtitleHardBurnFilterFixedPaths()
        {
            var str = FFMpegArguments
                .FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt
                    .WithVideoFilters(filterOptions => filterOptions
                        .HardBurnSubtitle(SubtitleHardBurnOptions
                            .Create(subtitlePath: @"sample( \ : [ ] , ' ).srt"))))
                .Arguments;

            Assert.AreEqual(@"-i ""input.mp4"" -vf ""subtitles='sample( \\ \: \[ \] \, '\\\'' ).srt'"" ""output.mp4""",
                str);
        }

        [TestMethod]
        public void Builder_BuildString_StartNumber()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt.WithStartNumber(50)).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -start_number 50 \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_Threads_1()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt.UsingThreads(50)).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -threads 50 \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_Threads_2()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt.UsingMultithreading(true)).Arguments;
            Assert.AreEqual($"-i \"input.mp4\" -threads {Environment.ProcessorCount} \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_Codec()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt.WithVideoCodec(VideoCodec.LibX264)).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -c:v libx264 \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_Codec_Override()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4").OutputToFile("output.mp4", true,
                opt => opt.WithVideoCodec(VideoCodec.LibX264).ForcePixelFormat("yuv420p")).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -c:v libx264 -pix_fmt yuv420p \"output.mp4\" -y", str);
        }

        [TestMethod]
        public void Builder_BuildString_Duration()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt.WithDuration(TimeSpan.FromSeconds(20))).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -t 00:00:20 \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_Raw()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4", false, opt => opt.WithCustomArgument(null!))
                .OutputToFile("output.mp4", false, opt => opt.WithCustomArgument(null!)).Arguments;
            Assert.AreEqual(" -i \"input.mp4\"  \"output.mp4\"", str);

            str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt.WithCustomArgument("-acodec copy")).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -acodec copy \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_ForcePixelFormat()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt.ForcePixelFormat("yuv444p")).Arguments;
            Assert.AreEqual("-i \"input.mp4\" -pix_fmt yuv444p \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_PanAudioFilterChannelNumber()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false,
                    opt => opt.WithAudioFilters(filterOptions => filterOptions.Pan(2, "c0=c1", "c1=c1")))
                .Arguments;

            Assert.AreEqual("-i \"input.mp4\" -af \"pan=2c|c0=c1|c1=c1\" \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_PanAudioFilterChannelLayout()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false,
                    opt => opt.WithAudioFilters(filterOptions => filterOptions.Pan("stereo", "c0=c0", "c1=c1")))
                .Arguments;

            Assert.AreEqual("-i \"input.mp4\" -af \"pan=stereo|c0=c0|c1=c1\" \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_PanAudioFilterChannelNoOutputDefinition()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false,
                    opt => opt.WithAudioFilters(filterOptions => filterOptions.Pan("stereo")))
                .Arguments;

            Assert.AreEqual("-i \"input.mp4\" -af \"pan=stereo\" \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_DynamicAudioNormalizerDefaultFormat()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false,
                    opt => opt.WithAudioFilters(filterOptions => filterOptions.DynamicNormalizer()))
                .Arguments;

            Assert.AreEqual("-i \"input.mp4\" -af \"dynaudnorm=f=500:g=31:p=0.95:m=10.0:r=0.0:n=1:c=0:b=0:s=0.0\" \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_DynamicAudioNormalizerWithValuesFormat()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false,
                    opt => opt.WithAudioFilters(filterOptions => filterOptions.DynamicNormalizer(125, 13, 0.9215, 5.124, 0.5458, false, true, true, 0.3333333)))
                .Arguments;

            Assert.AreEqual("-i \"input.mp4\" -af \"dynaudnorm=f=125:g=13:p=0.92:m=5.1:r=0.5:n=0:c=1:b=1:s=0.3\" \"output.mp4\"", str);
        }

        [TestMethod]
        public void Builder_BuildString_Audible_AAXC_Decryption()
        {
            var str = FFMpegArguments.FromFileInput("input.aaxc", false, x => x.WithAudibleEncryptionKeys("123", "456"))
                .MapMetaData()
                .OutputToFile("output.m4b", true, x => x.WithTagVersion(3).DisableChannel(Channel.Video).CopyChannel(Channel.Audio))
                .Arguments;

            Assert.AreEqual("-audible_key 123 -audible_iv 456 -i \"input.aaxc\" -map_metadata 0 -id3v2_version 3 -vn -c:a copy \"output.m4b\" -y", str);
        }

        [TestMethod]
        public void Builder_BuildString_PadFilter()
        {
            var str = FFMpegArguments
                .FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt
                    .WithVideoFilters(filterOptions => filterOptions
                        .Pad(PadOptions
                            .Create("max(iw,ih)", "ow")
                            .WithParameter("x", "(ow-iw)/2")
                            .WithParameter("y", "(oh-ih)/2")
                            .WithParameter("color", "violet")
                            .WithParameter("eval", "frame"))))
                .Arguments;

            Assert.AreEqual(
                "-i \"input.mp4\" -vf \"pad=width=max(iw\\,ih):height=ow:x=(ow-iw)/2:y=(oh-ih)/2:color=violet:eval=frame\" \"output.mp4\"",
                str);
        }

        [TestMethod]
        public void Builder_BuildString_PadFilter_Alt()
        {
            var str = FFMpegArguments
                .FromFileInput("input.mp4")
                .OutputToFile("output.mp4", false, opt => opt
                    .WithVideoFilters(filterOptions => filterOptions
                        .Pad(PadOptions
                            .Create("4/3")
                            .WithParameter("x", "(ow-iw)/2")
                            .WithParameter("y", "(oh-ih)/2")
                            .WithParameter("color", "violet")
                            .WithParameter("eval", "frame"))))
                .Arguments;

            Assert.AreEqual(
                "-i \"input.mp4\" -vf \"pad=aspect=4/3:x=(ow-iw)/2:y=(oh-ih)/2:color=violet:eval=frame\" \"output.mp4\"",
                str);
        }

        [TestMethod]
        public void Builder_BuildString_GifPalette()
        {
            var streamIndex = 0;
            var size = new Size(640, 480);

            var str = FFMpegArguments
                .FromFileInput("input.mp4")
                .OutputToFile("output.gif", false, opt => opt
                    .WithGifPaletteArgument(streamIndex, size))
                .Arguments;

            Assert.AreEqual($"""
                -i "input.mp4" -filter_complex "[0:v] fps=12,scale=w={size.Width}:h={size.Height},split [a][b];[a] palettegen=max_colors=32 [p];[b][p] paletteuse=dither=bayer" "output.gif"
                """, str);
        }

        [TestMethod]
        public void Builder_BuildString_GifPalette_NullSize_FpsSupplied()
        {
            var streamIndex = 1;

            var str = FFMpegArguments
                .FromFileInput("input.mp4")
                .OutputToFile("output.gif", false, opt => opt
                    .WithGifPaletteArgument(streamIndex, null, 10))
                .Arguments;

            Assert.AreEqual($"""
                -i "input.mp4" -filter_complex "[{streamIndex}:v] fps=10,split [a][b];[a] palettegen=max_colors=32 [p];[b][p] paletteuse=dither=bayer" "output.gif"
                """, str);
        }

        [TestMethod]
        public void Builder_BuildString_MultiOutput()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .MultiOutput(args => args
                    .OutputToFile("output.mp4", overwrite: true, args => args.CopyChannel())
                    .OutputToFile("output.ts", overwrite: false, args => args.CopyChannel().ForceFormat("mpegts"))
                    .OutputToUrl("http://server/path", options => options.ForceFormat("webm")))
                    .Arguments;
            Assert.AreEqual($"""
                -i "input.mp4" -c:a copy -c:v copy "output.mp4" -y -c:a copy -c:v copy -f mpegts "output.ts" -f webm http://server/path
                """, str);
        }

        [TestMethod]
        public void Builder_BuildString_MBROutput()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .MultiOutput(args => args
                    .OutputToFile("sd.mp4", overwrite: true, args => args.Resize(1200, 720))
                    .OutputToFile("hd.mp4", overwrite: false, args => args.Resize(1920, 1080)))
                    .Arguments;
            Assert.AreEqual($"""
                -i "input.mp4" -s 1200x720 "sd.mp4" -y -s 1920x1080 "hd.mp4"
                """, str);
        }

        [TestMethod]
        public void Builder_BuildString_TeeOutput()
        {
            var str = FFMpegArguments.FromFileInput("input.mp4")
                .OutputToTee(args => args
                    .OutputToFile("output.mp4", overwrite: false, args => args.WithFastStart())
                    .OutputToUrl("http://server/path", options => options.ForceFormat("mpegts").SelectStream(0, channel: Channel.Video)))
                    .Arguments;
            Assert.AreEqual($"""
                -i "input.mp4" -f tee "[movflags=faststart]output.mp4|[f=mpegts:select=\'0:v:0\']http://server/path"
                """, str);
        }
        [TestMethod]
        public void Builder_BuildString_MultiInput()
        {
            var audioStreams = string.Join("", _multiFiles.Select((item, index) => $"[{index}:0]"));
            var mixFilter = $"{audioStreams}amix=inputs={_multiFiles.Length}:duration=longest:dropout_transition=1:normalize=0[final]";
            var ffmpegArgs = $"-filter_complex \"{mixFilter}\" -map \"[final]\"";
            var str = FFMpegArguments
                .FromFileInput(_multiFiles)
                .OutputToFile("output.mp3", overwrite: true, options => options
                    .WithCustomArgument(ffmpegArgs)
                    .WithAudioCodec(AudioCodec.LibMp3Lame) // Set the audio codec to MP3
                    .WithAudioBitrate(128) // Set the bitrate to 128kbps
                    .WithAudioSamplingRate(48000) // Set the sample rate to 48kHz
                    .WithoutMetadata() // Remove metadata
                    .WithCustomArgument("-ac 2 -write_xing 0 -id3v2_version 0")) // Force 2 Channels
                .Arguments;
            Assert.AreEqual($"-i \"1.mp3\" -i \"2.mp3\" -i \"3.mp3\" -i \"4.mp3\" -filter_complex \"[0:0][1:0][2:0][3:0]amix=inputs=4:duration=longest:dropout_transition=1:normalize=0[final]\" -map \"[final]\" -c:a libmp3lame -b:a 128k -ar 48000 -map_metadata -1 -ac 2 -write_xing 0 -id3v2_version 0 \"output.mp3\" -y", str);
        }
        [TestMethod]
        public void Pre_VerifyExists_AllFilesExist()
        {
            // Arrange
            var filePaths = new List<string>
            {
                Path.GetTempFileName(),
                Path.GetTempFileName(),
                Path.GetTempFileName()
            };
            var argument = new MultiInputArgument(true, filePaths);
            try
            {
                // Act & Assert
                argument.Pre(); // No exception should be thrown
            }
            finally
            {
                // Cleanup
                foreach (var filePath in filePaths)
                {
                    File.Delete(filePath);
                }
            }
        }

        [TestMethod]
        public void Pre_VerifyExists_SomeFilesNotExist()
        {
            // Arrange
            var filePaths = new List<string>
            {
                Path.GetTempFileName(),
                "file2.mp4",
                "file3.mp4"
            };
            var argument = new MultiInputArgument(true, filePaths);
            try
            {
                // Act & Assert
                Assert.ThrowsException<FileNotFoundException>(() => argument.Pre());
            }
            finally
            {
                // Cleanup
                File.Delete(filePaths[0]);
            }
        }

        [TestMethod]
        public void Pre_VerifyExists_NoFilesExist()
        {
            // Arrange
            var filePaths = new List<string>
            {
                "file1.mp4",
                "file2.mp4",
                "file3.mp4"
            };
            var argument = new MultiInputArgument(true, filePaths);
            // Act & Assert
            Assert.ThrowsException<FileNotFoundException>(() => argument.Pre());
        }
    }
}
