using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public static class AudioTool
    {
        public static float[] WavData2ClipData(byte[] audioData)
        {
            int headerSize = 44; // Standard WAV header is 44 bytes
            int subchunk1Size = BitConverter.ToInt32(audioData, 16);
            int subchunk2Size = BitConverter.ToInt32(audioData, 40);
            int dataSize = subchunk2Size;

            float[] clipData = new float[dataSize / 2];
            for (int i = headerSize; i < headerSize + dataSize; i += 2)
            {
                clipData[(i - headerSize) / 2] = (short)((audioData[i + 1] << 8) | audioData[i]) / 32768.0f;
            }

            return clipData;
        }

        public static byte[] ClipData2WavData(float[] clipData)
        {
            short[] intData = new short[clipData.Length];
            byte[] byteData = new byte[clipData.Length * 2];
            float rescaleFactor = 32768.0f;
            for (int i = 0; i < clipData.Length; i++)
            {
                intData[i] = (short)(clipData[i] * rescaleFactor);
                var byteArr = BitConverter.GetBytes(intData[i]);
                byteArr.CopyTo(byteData, i * 2);
            }

            return byteData;
        }
    }
}
