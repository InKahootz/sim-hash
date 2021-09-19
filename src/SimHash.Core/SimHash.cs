using System;
using System.Collections.Generic;
using System.Data.HashFunction;
using System.Data.HashFunction.Jenkins;
using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace SimHashLib
{

    public class SimHash
    {
        public enum HashingType
        {
            MD5,
            Jenkins
        }
        public int fpSize = 64;

        public ulong value { get; set; }

        public SimHash()
        { }

        public SimHash(HashingType hashingType)
        {
            hashAlgorithm = hashingType;
        }

        public SimHash(SimHash simHash)
        {
            value = simHash.value;
        }

        public SimHash(ulong fingerPrint)
        {
            value = fingerPrint;
        }

        public void GenerateSimhash(string content)
        {
            var shingling = new Shingling();
            var shingles = shingling.Tokenize(content);
            GenerateSimhash(shingles);
        }

        private HashingType hashAlgorithm = HashingType.Jenkins;

        public void GenerateSimhash(List<string> features)
        {
            switch (hashAlgorithm)
            {
                case HashingType.MD5:
                    MD5Compute(features);
                    break;
                default:
                    JenkinsCompute(features);
                    break;

            }
        }

        public long GetFingerprintAsLong()
        {
            return (long)value;
        }

        public int Distance(SimHash another)
        {
            if (fpSize != another.fpSize) throw new Exception();
            ulong x = (value ^ another.value) & (ulong.MaxValue);
            int ans = 0;
            while (x > 0)
            {
                ans++;
                x &= x - 1;
            }
            return ans;
        }
        private void JenkinsCompute(List<string> features)
        {
            int[] v = SetupFingerprint();
            ulong[] masks = setupMasks();

            foreach (string feature in features)
            {
                ulong h = ComputeJenkinsHash(feature);
                int w = 1;
                for (int i = 0; i < fpSize; i++)
                {
                    ulong result = h & masks[i];
                    v[i] += (result > 0) ? w : -w;
                }
            }

            value = Fingerprint(v, masks);
        }

        private void MD5Compute(List<string> features)
        {
            int[] v = SetupFingerprint();
            ulong[] masks = setupMasks();

            foreach (string feature in features)
            {
                //this is using MD5 which is REALLY slow
                BigInteger h = ComputeMD5Hash(feature);
                int w = 1;
                for (int i = 0; i < fpSize; i++)
                {
                    //convert to BigInt so we can use BitWise
                    BigInteger bMask = masks[i];
                    BigInteger result = h & bMask;
                    v[i] += (result > 0) ? w : -w;
                }
            }

            value = Fingerprint(v, masks);
        }


        private ulong Fingerprint(int[] v, ulong[] masks)
        {
            ulong ans = 0;
            for (int i = 0; i < fpSize; i++)
            {
                if (v[i] >= 0)
                {
                    ans |= masks[i];
                }
            }
            return ans;
        }

        private int[] SetupFingerprint()
        {
            int[] v = new int[fpSize];
            for (int i = 0; i < v.Length; i++) v[i] = 0;
            return v;
        }

        private ulong[] setupMasks()
        {
            ulong[] masks = new ulong[fpSize];
            for (int i = 0; i < masks.Length; i++)
            {
                masks[i] = (ulong)1 << i;
            }
            return masks;
        }

        public ulong ComputeJenkinsHash(string x)
        {
            var jenkinsLookup3 = JenkinsLookup3Factory.Instance.Create(new JenkinsLookup3Config() { HashSizeInBits = 64 });
            var resultBytes = jenkinsLookup3.ComputeHash(x);

            var y = BitConverter.ToUInt64(resultBytes.Hash, 0);

            return y;
        }

        private BigInteger ComputeMD5Hash(string x)
        {
            string hexValue = ComputeMD5HashAsHex(x);
            BigInteger b = HexToBigIntegter(hexValue);
            return b;
        }
        public string ComputeMD5HashAsHex(string x)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(x));

                string returnString = "";
                for (int i = 0; i < data.Length; i++)
                {
                    returnString += data[i].ToString("x2");
                }
                return returnString;
            }
        }

        public BigInteger HexToBigIntegter(string x)
        {
            BigInteger bigNumber = BigInteger.Parse(x, NumberStyles.AllowHexSpecifier);
            return bigNumber;
        }
    }
}
