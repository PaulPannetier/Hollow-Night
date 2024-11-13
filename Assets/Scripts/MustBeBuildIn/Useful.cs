#region using

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

#endregion

#region Save

public static class Save
{
    /// <summary>
    /// Convert any Serializable object in JSON string.
    /// </summary>
    /// <param name="obj">The object to serialize</param>
    /// <returns> A string represent the object in parameter</returns>
    public static string Serialize(object obj, bool withIndentation = false) => JsonUtility.ToJson(obj, withIndentation);
    /// <summary>
    /// Convert any string reprensent a Serializable object to the object.
    /// </summary>
    /// <typeparam name="T">The type of the object return</typeparam>
    /// <param name="JSONString">The string represent the object return</param>
    /// <returns> A Serializable object describe by the string in parameter</returns>
    public static T Deserialize<T>(string JSONString) => JsonUtility.FromJson<T>(JSONString);
    
    /// <summary>
    /// Write in the customer machine a file with the object inside
    /// </summary>
    /// <param name="objToWrite">The object to save</param>
    /// <param name="filename">the save path, begining to the game's folder</param>
    /// <param name="withIndentation">Weather to format the output for readability (indent).</param>
    /// <param name="mkdir">Weather to automatically create the directory path.</param>
    /// <returns> true if the save complete successfully, false overwise</returns>
    public static bool WriteJSONData(object objToWrite, string fileName, bool withIndentation = false, bool mkdir=false)
    {
        try
        {
            string s = Serialize(objToWrite, withIndentation);
            if (s == "{}")
                return false;
			if (mkdir)
				Directory.CreateDirectory(Application.dataPath + Path.GetDirectoryName(fileName));
            File.WriteAllText(Application.dataPath + fileName, s);
        }
        catch
        {
            return false;
        }
        return true;
    }

    /// <summary>
    /// Write in the customer machine a file with the object inside asynchronously
    /// </summary>
    /// <param name="objToWrite">The object to save</param>
    /// <param name="filename">the save path, begining to the game's folder</param>
    /// <param name="callback">The callback when the function end</param>
    /// <returns> true if the save complete successfully, false overwise</returns>
    public static async Task<bool> WriteJSONDataAsync(object objToWrite, string fileName, Action<bool> callback, bool withIndentation = false)
    {
        try
        {
            string s = Serialize(objToWrite, withIndentation);
            if (s == "{}")
            {
                callback?.Invoke(false);
                return false;
            }
            await File.WriteAllTextAsync(Application.dataPath + fileName, s);
            callback?.Invoke(true);
        }
        catch
        {
            callback?.Invoke(false);
            return false;
        }
        return true;
    }

    /// <typeparam name="T">The object to read's type</typeparam>
    /// <param name="fileName">The path of the file, begining to the game's folder</param>
    /// <param name="objRead"></param>
    /// <returns> true if the function complete successfully, false overwise</returns>
    public static bool ReadJSONData<T>(string fileName, out T objRead)
    {
        try
        {
            string jsonString = File.ReadAllText(Application.dataPath + fileName);
            if (jsonString == "{}")
            {
                objRead = default(T);
                return false;
            }
            objRead = Deserialize<T>(jsonString);
            return true;
        }
        catch (Exception)
        {
            objRead = default(T);
            return false;
        }        
    }

    /// <typeparam name="T">The object to read's type</typeparam>
    /// <param name="fileName">The path of the file, begining to the game's folder</param>
    /// <param name="objRead"></param>
    /// <returns> true if the function complete successfully, false overwise</returns>
    public static async Task<bool> ReadJSONDataAsync<T>(string fileName, Action<bool, T> callback)
    {
        try
        {
            string jsonString = await File.ReadAllTextAsync(Application.dataPath + fileName);
            if (jsonString == "{}")
            {
                callback?.Invoke(false, default(T));
                return false;
            }
            callback?.Invoke(true, Deserialize<T>(jsonString));
            return true;
        }
        catch (Exception)
        {
            callback?.Invoke(false, default(T));
            return false;
        }
    }

    public static async Task WriteStringAsync(string data, string fileName, Action<bool> callback, bool append = true)
    {
        await File.WriteAllTextAsync(Application.dataPath + fileName, data);
    }

    public static void ImageInPNGFormat(Color[] pixels, int w, int h, string path, string name, FilterMode filterMode = FilterMode.Point, TextureWrapMode textureWrapMode = TextureWrapMode.Clamp)
    {
        Texture2D texture = GenerateImage(pixels, w, h, filterMode, textureWrapMode);
        File.WriteAllBytes(Application.dataPath + path + name + @".png", texture.EncodeToPNG());
    }

    public static void ImageInPNGFormat(Texture2D texture, string path, string name)
    {
        File.WriteAllBytes(Application.dataPath + path + name + @".png", texture.EncodeToPNG());
    }

    public static void ImageInJPGFormat(Color[] pixels, int w, int h, string path, string name, FilterMode filterMode = FilterMode.Point, TextureWrapMode textureWrapMode = TextureWrapMode.Clamp)
    {
        Texture2D texture = GenerateImage(pixels, w, h, filterMode, textureWrapMode);
        File.WriteAllBytes(Application.dataPath + path + name + @".jpg", texture.EncodeToJPG());
    }

    public static void ImageInJPGFormat(Texture2D texture, string path, string name)
    {
        File.WriteAllBytes(Application.dataPath + path + name + @".jpg", texture.EncodeToJPG());
    }

    private static Texture2D GenerateImage(Color[] pixels, int w, int h, FilterMode filterMode = FilterMode.Point, TextureWrapMode textureWrapMode = TextureWrapMode.Clamp)
    {
        Texture2D img = new Texture2D(w, h, TextureFormat.RGBAFloat, false);
        img.SetPixelData(pixels, 0);
        img.filterMode = filterMode;
        img.wrapMode = textureWrapMode;
        img.Apply();
        return img;
    }
}

#endregion

#region Random

public static class Random
{
    private static System.Random random = new System.Random();
    private static readonly float twoPi = 2f * Mathf.PI;

    #region Seed

    public static void SetSeed(int seed)
    {
        random = new System.Random(seed);
        Noise2d.Reseed();
    }
    /// <summary>
    /// randomize de seed of the random, allow to have diffenrent random number at each launch of the game
    /// </summary>
    public static void SetRandomSeed()
    {
        SetSeed(Environment.TickCount);
    }

    #endregion

    #region Random Value and vector

    /// <returns> A random integer between a and b, [|a, b|]</returns>
    public static int Rand(int a, int b) => random.Next(a, b + 1);
    /// <returns> A random float between 0 and 1, [0, 1]</returns>
    public static float Rand() => (float)random.Next(int.MaxValue) / (int.MaxValue - 1);
    /// <returns> A random float between a and b, [a, b]</returns>
    public static float Rand(float a, float b) => Rand() * Mathf.Abs(b - a) + a;
    /// <returns> A random int between a and b, [|a, b|[</returns>
    public static int RandExclude(int a, int b) => random.Next(a, b);
    /// <returns> A random double between a and b, [a, b[</returns>
    public static float RandExclude(float a, float b) => (float)random.NextDouble() * (Mathf.Abs(b - a)) + a;
    public static float RandExclude() => (float)random.NextDouble();
    public static float PerlinNoise(float x, float y) => Noise2d.Noise(x, y);
    public static Color Color() => new Color(Rand(), Rand(), Rand(), 1f);
    /// <returns> A random color with a random opacity</returns>
    public static Color ColorTransparent() => new Color(Rand(0, 255) / 255f, Rand(0, 255) / 255f, Rand(0, 255) / 255f, (float)random.NextDouble());
    /// <returns> A random Vector2 normalised</returns>
    public static Vector2 Vector2()
    {
        float angle = RandExclude(0f, twoPi);
        return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    }
    /// <returns> A random Vector2 with de magnitude in param</returns>
    public static Vector2 Vector2(float magnitude)
    {
        float angle = RandExclude(0f, twoPi);
        return new Vector2(magnitude * Mathf.Cos(angle), magnitude * Mathf.Sin(angle));
    }
    /// <returns> A random Vector2 with a randoml magnitude</returns>
    public static Vector2 Vector2(float minMagnitude, float maxMagnitude)
    {
        float angle = RandExclude(0f, twoPi);
        float magnitude = Rand(minMagnitude, maxMagnitude);
        return new Vector2(magnitude * Mathf.Cos(angle), magnitude * Mathf.Sin(angle));
    }
    /// <returns> A random Vector3 normalised</returns>
    public static Vector3 Vector3()
    {
        float teta = Rand(0f, twoPi);
        float phi = RandExclude(0f, twoPi);
        return new Vector3(Mathf.Sin(teta) * Mathf.Cos(phi), Mathf.Sin(teta) * Mathf.Sin(phi), Mathf.Cos(teta));
    }
    /// <returns> A random Vector3 with de magnitude in param</returns>
    public static Vector3 Vector3(float magnitude)
    {
        float teta = Rand(0f, Mathf.PI);
        float phi = RandExclude(0f, twoPi);
        return new Vector3(magnitude * Mathf.Sin(teta) * Mathf.Cos(phi), magnitude * Mathf.Sin(teta) * Mathf.Sin(phi), magnitude * Mathf.Cos(teta));
    }
    /// <returns> A random Vector3 with a random magnitude</returns>
    public static Vector3 Vector3(float minMagnitude, float maxMagnitude)
    {
        float teta = Rand(0f, Mathf.PI);
        float phi = RandExclude(0f, twoPi);
        float magnitude = Rand(minMagnitude, maxMagnitude);
        return new Vector3(magnitude * Mathf.Sin(teta) * Mathf.Cos(phi), magnitude * Mathf.Sin(teta) * Mathf.Sin(phi), magnitude * Mathf.Cos(teta));
    }

    public static Vector2 PointInCircle(CircleCollider2D circle) => PointInCircle(circle.transform.position, circle.radius);
    public static Vector2 PointInCircle(in Vector2 center, float radius)
    {
        float x, y;
        while(true)
        {
            x = Rand() * 2f * radius - radius;
            y = Rand() * 2f * radius - radius;
            if(x * x + (y * y) <= radius * radius)
                return new Vector2(center.x + x, center.y + y);
        }
    }
    public static Vector2 PointInRectangle(BoxCollider2D rec) => PointInRectangle(rec.transform.position, rec.size);
    public static Vector2 PointInRectangle(in Vector2 center, in Vector2 size)
    {
        return new Vector2(center.x + (Rand() - 0.5f) * size.x, center.y + (Rand() - 0.5f) * size.y);
    }

    #endregion

    #region Noise

    private static class Noise2d
    {
        private static int[] _permutation;

        private static readonly Vector2[] _gradients;

        static Noise2d()
        {
            CalculatePermutation(out _permutation);
            CalculateGradients(out _gradients);
        }

        private static void CalculatePermutation(out int[] p)
        {
            p = Enumerable.Range(0, 256).ToArray();

            /// shuffle the array
            for (var i = 0; i < p.Length; i++)
            {
                var source = RandExclude(0, p.Length);

                var t = p[i];
                p[i] = p[source];
                p[source] = t;
            }
        }

        /// <summary>
        /// generate a new permutation.
        /// </summary>
        public static void Reseed()
        {
            CalculatePermutation(out _permutation);
        }

        private static void CalculateGradients(out Vector2[] grad)
        {
            grad = new Vector2[256];

            for (var i = 0; i < grad.Length; i++)
            {
                Vector2 gradient;
                do
                {
                    gradient = new Vector2((RandExclude() * 2f - 1f), (RandExclude() * 2f - 1f));
                }
                while (gradient.SqrMagnitude() >= 1);

                gradient.Normalize();

                grad[i] = gradient;
            }
        }

        private static float Drop(float t)
        {
            t = Mathf.Abs(t);
            return 1f - t * t * t * (t * (t * 6 - 15) + 10);
        }

        private static float Q(float u, float v)
        {
            return Drop(u) * Drop(v);
        }

        public static float Noise(float x, float y)
        {
            Vector2 cell = new Vector2((float)Mathf.Floor(x), (float)Mathf.Floor(y));

            float total = 0f;

            Vector2[] corners = new[] { new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 0f), new Vector2(1f, 1f) };

            foreach (var n in corners)
            {
                Vector2 ij = cell + n;
                Vector2 uv = new Vector2(x - ij.x, y - ij.y);

                int index = _permutation[(int)ij.x % _permutation.Length];
                index = _permutation[(index + (int)ij.y) % _permutation.Length];

                Vector2 grad = _gradients[index % _gradients.Length];

                total += Q(uv.x, uv.y) * grad.Dot(uv);
            }
            return Mathf.Max(Mathf.Min(total, 1f), -1f);
        }
    }

    #endregion

    #region Proba laws

    //Generer les lois de probas ! fonction non tester
    public static int Bernoulli(float p) => Rand() <= p ? 1 : 0;
    public static int Binomial(int n, int p)
    {
        int count = 0;
        for (int i = 0; i < n; i++)
            count += Bernoulli(p);
        return count;
    }
    public static float Expodential(float lambda) => (-1f / lambda) * Mathf.Log(Rand());
    public static int Poisson(float lambda)
    {
        float x = Rand();
        int n = 0;
        while (x > Mathf.Exp(-lambda))
        {
            x *= Rand();
            n++;
        }
        return n;
    }
    public static int Geometric(float p)
    {
        int count = 0;
        do
        {
            count++;
        } while (Bernoulli(p) == 0);
        return count;
    }
    private static float N01() => Mathf.Sqrt(-2f * Mathf.Log(Rand())) * Mathf.Cos(twoPi * Rand());
    public static float Normal(float esp, float sigma) => N01() * sigma + esp;

    #endregion

    #region Shuffle

    public static void Shuffle<T>(this List<T> list)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            int j = Random.Rand(0, i);
            T tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }

    public static void Shuffle<T>(this T[] list)
    {
        for (int i = list.Length - 1; i >= 0; i--)
        {
            int j = Random.Rand(0, i);
            T tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }

    #endregion
}

#endregion

#region ICloneable<T>

public interface ICloneable<T>
{
    public T Clone();
}

#endregion

#region Useful

public static class Useful
{
    #region Colordeeper/Lighter

    /// <summary>
    /// Return a Color deeper than the color in argument
    /// </summary>
    /// <param name="c">The color to change</param>
    /// <param name="percent">le coeff €[0, 1] d'assombrissement</param>
    /// <returns></returns>
    public static Color ColorDeeper(in Color c, float coeff) => new Color(c.r * (1f - coeff), c.g * (1f - coeff), c.b * (1f - coeff), c.a);
    /// <summary>
    /// Return a Color lighter than the color in argument
    /// </summary>
    /// <param name="c">The color to change</param>
    /// <param name="percent">le coeff €[0, 1] de luminosité</param>
    /// <returns></returns>
    public static Color ColorLighter(in Color c, float coeff) => new Color(((1f - c.r) * coeff) + c.r, ((1f - c.g) * coeff) + c.g, ((1f - c.b) * coeff) + c.b, c.a);

    public static Color ColorRgbFromTemperature(float temperature)
    {
        // Temperature must fit between 1000 and 40000 degrees.
        temperature = Mathf.Clamp(temperature, 1000, 40000);

        // All calculations require temperature / 100, so only do the conversion once.
        temperature *= 0.01f;

        // Compute each color in turn.
        int red, green, blue;

        // First: red.
        if (temperature <= 66)
        {
            red = 255;
        }
        else
        {
            // Note: the R-squared value for this approximation is 0.988.
            red = (int)(329.698727446 * (Math.Pow(temperature - 60, -0.1332047592)));
            red = Mathf.Clamp(red, 0, 255);
        }

        // Second: green.
        if (temperature <= 66)
        {
            // Note: the R-squared value for this approximation is 0.996.
            green = (int)(99.4708025861 * Math.Log(temperature) - 161.1195681661);
        }
        else
        {
            // Note: the R-squared value for this approximation is 0.987.
            green = (int)(288.1221695283 * (Math.Pow(temperature - 60, -0.0755148492)));
        }

        green = Mathf.Clamp(green, 0, 255);

        // Third: blue.
        if (temperature >= 66)
        {
            blue = 255;
        }
        else if (temperature <= 19)
        {
            blue = 0;
        }
        else
        {
            // Note: the R-squared value for this approximation is 0.998.
            blue = (int)(138.5177312231 * Math.Log(temperature - 10) - 305.0447927307);
            blue = Mathf.Clamp(blue, 0, 255);
        }
        return new Color(red * 0.00392156862f, green * 0.00392156862f, blue * 0.00392156862f);//0.00392156862f = 1f/255f
    }

    public static Texture2D Lerp(Texture2D A, Texture2D B, float t)
    {
        if (A.width != B.width || A.height != B.height)
        {
            int w = Mathf.Min(A.width, B.width);
            int h = Mathf.Min(A.height, B.height);
            if (A.width < B.width || A.height < B.height)
                A.Reinitialize(w, h);
            if (B.width < A.width || B.height < A.height)
                B.Reinitialize(w, h);
        }
        Texture2D texture = new Texture2D(A.width, A.height);
        for (int x = 0; x < A.width; x++)
        {
            for (int y = 0; y < A.height; y++)
            {
                texture.SetPixel(x, y, Color.Lerp(A.GetPixel(x, y), B.GetPixel(x, y), t));
            }
        }
        return texture;
    }

    #endregion

    #region Vector and Maths

    //Vector2
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float SqrDistance(in this Vector2 v, in Vector2 a) => (a.x - v.x) * (a.x - v.x) + (a.y - v.y) * (a.y - v.y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Distance(in this Vector2 v, in Vector2 a) => Mathf.Sqrt(v.SqrDistance(a));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCollinear(this Vector2 a, in Vector2 v) => Mathf.Abs((v.x / a.x) - (v.y / a.y)) < 1e-3f;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 Cross(in this Vector2 v1, in Vector2 v) => new Vector3(0f, 0f, v1.x * v.y - v1.y * v.x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Dot(in this Vector2 v1, in Vector2 v) => v1.x * v.x + v1.y * v.y;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Vector2FromAngle(float angle) => new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 Vector2FromAngle(float angle, float length) => new Vector2(length * Mathf.Cos(angle), length * Mathf.Sin(angle));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 ToVector3(in this Vector2 v) => new Vector3(v.x, v.y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4 ToVector4(in this Vector2 v) => new Vector4(v.x, v.y);
    /// <returns>the orthogonal normalised vector of v</returns>
    public static Vector2 NormalVector(in this Vector2 v)
    {
        if (!Mathf.Approximately(v.x, 0f))
        {
            float y = Mathf.Sqrt(1f / (((v.y * v.y) / (v.x * v.x)) + 1f));
            return new Vector2(-v.y * y / v.x, y);
        }
        else if (!Mathf.Approximately(v.y, 0f))
        {
            float x = Mathf.Sqrt(1f / (1f + (v.x * v.x) / (v.y * v.y)));
            return new Vector2(x, -v.x * x / v.y);
        }
        else
        {
            return v;
        }
    }

    //Vector3
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float SqrDistance(in this Vector3 v, in Vector3 a) => (a.x - v.x) * (a.x - v.x) + (a.y - v.y) * (a.y - v.y) + (a.z - v.z) * (a.z - v.z);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Distance(in this Vector3 v, in Vector3 a) => Mathf.Sqrt(v.SqrDistance(a));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsCollinear(this Vector3 a, in Vector3 b) => Mathf.Abs(b.x / a.x - b.y / a.y) < 0.007f * Mathf.Abs(b.y / a.y) &&
                                                                        Mathf.Abs(b.x / a.x - b.z / a.z) < 0.007f * Mathf.Abs(b.z / a.z) &&
                                                                        Mathf.Abs(b.y / a.y - b.z / a.z) < 0.007f * Mathf.Abs(b.z / a.z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 Cross(in this Vector3 v1, in Vector3 v) => new Vector3(v1.y * v.z - v1.z * v.y, v1.z * v.x - v1.x * v.z, v1.x * v.y - v1.y * v.x);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Dot(in this Vector3 v1, in Vector3 v) => v1.x * v.x + v1.y * v.y + v1.z * v.z;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 NormalVector(in this Vector3 v1, in Vector3 v) => v1.Cross(v).normalized;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 ToVector2(in this Vector3 v) => new Vector2(v.x, v.y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector4 ToVector4(in this Vector3 v) => new Vector4(v.x, v.y);

    //Vector4
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float SqrDistance(in this Vector4 v, in Vector4 a) => (a.x - v.x) * (a.x - v.x) + (a.y - v.y) * (a.y - v.y) + (a.z - v.z) * (a.z - v.z) + (a.w - v.w) * (a.w - v.w);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Distance(in this Vector4 v, in Vector4 a) => Mathf.Sqrt(v.SqrDistance(a));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2 ToVector2(in this Vector4 v) => new Vector2(v.x, v.y);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3 ToVector3(in this Vector4 v) => new Vector3(v.x, v.y, v.z);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="a">le point de début du vecteur</param>
    /// <param name="b">le point de fin du vecteur</param>
    /// <returns>l'angle en rad entre 0 et 2pi entre le vecteur (1,0) et (b-a) </returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float AngleHori(this in Vector2 a, in Vector2 b) => Mathf.Atan2(a.y - b.y, a.x - b.x) + Mathf.PI;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Angle(this in Vector2 a, in Vector2 b) => ClampModulo(-Mathf.PI, Mathf.PI, AngleHori(Vector2.zero, a) + AngleHori(Vector2.zero, b));
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Sign(this float a) => Mathf.Approximately(a, 0f) ? 0f : (a > 0f ? 1f : -1f);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Sign(this int a) => a == 0 ? 0 : (a > 0 ? 1 : -1);

    /// <summary>
    /// Renvoie l'angle minimal entre le segment [ca] et [cb]
    /// </summary>
    public static float Angle(in Vector2 c, in Vector2 a, in Vector2 b)
    {
        float ang1 = AngleHori(c, a);
        float ang2 = AngleHori(c, b);
        float diff = Mathf.Abs(ang1 - ang2);
        return Mathf.Min(diff, 2f * Mathf.PI - diff);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float WrapAngle(float angle) => ClampModulo(0f, 2f * Mathf.PI, angle);

    public static float AngleDist(float a1, float a2)
    {
        a1 = WrapAngle(a1);
        a2 = WrapAngle(a2);
        return Mathf.Min(Mathf.Abs(a1 - a2), Mathf.Abs(Mathf.Abs(a1 - a2) - 2f * Mathf.PI));
    }

    /// <returns> a like a = value % (end -  start) + start, a€[start, end[ /returns>
    public static float ClampModulo(float start, float end, float value)
    {
        if (end < start)
            return ClampModulo(end, start, value);
        if (end - start < Mathf.Epsilon)
            return start;

        if (value < end && value >= start)
            return value;
        else
        {
            float modulo = end - start;
            float result = ((value - start) % modulo) + start;
            if (result >= end)
                return result - modulo;
            if (result < start)
                return result + modulo;
            return result;
        }
    }

    /// <returns> a like a = value % (end -  start) + start, a€[start, end[ /returns>
    public static int ClampModulo(int start, int end, int value)
    {
        if (end < start)
            return ClampModulo(end, start, value);
        if (end == start)
            return start;

        if (value < end && value >= start)
            return value;
        else
        {
            int modulo = end - start;
            int result = ((value - start) % modulo) + start;
            if (result >= end)
                return result - modulo;
            if (result < start)
                return result + modulo;
            return result;
        }
    }

    /// <summary>
    /// Renvoie si pour aller de l'angle 1 vers l'angle 2 le plus rapidement il faut tourner à droite ou à gauche, ang€[0, 2pi[
    /// </summary>
    public static void DirectionAngle(float ang1, float ang2, out bool right)
    {
        float diff = Mathf.Abs(ang1 - ang2);
        float angMin = Mathf.Min(diff, 2f * Mathf.PI - diff);
        right = Mathf.Abs((ang1 + angMin) % (2f * Mathf.PI) - ang2) < 0.1f;
    }
    /// <summary>
    /// Renvoie la valeur arrondi de n
    /// </summary>
    public static int Round(this float n) => (n - Mathf.Floor(n)) >= 0.5f ? (int)n + 1 : (int)n;
    /// <summary>
    /// Renvoie la valeur arrondi de n au nombre de décimales en param, ex : Round(51.6854, 2) => 51.69
    /// </summary>
    public static float Round(this float n, int nbDecimals)
    {
        float npow = n * Mathf.Pow(10f, nbDecimals);
        return npow - (int)npow >= 0.5f ? (((int)(npow + 1)) / Mathf.Pow(10f, nbDecimals)) : (((int)npow) / Mathf.Pow(10f, nbDecimals));
    }
    public static int Round(this double n) => (int)Math.Round(n);
    public static int Round(this double n, int nbDecimals) => (int)Math.Round(n, nbDecimals);
    public static int Floor(this float n) => Mathf.FloorToInt(n);
    public static int Ceil(this float n) => Mathf.CeilToInt(n);
    public static int Floor(this double n) => (int)Math.Floor(n);
    public static int Ceil(this double n) => (int)Math.Ceiling(n);

    public static bool Approximately(this float a, float b)
    {
        return MathF.Abs(b - a) < 1e-5f * MathF.Max(MathF.Pow(10f, MathF.Ceiling(MathF.Log10(MathF.Max(a, b)))), 1f);
    }

    public static bool Approximately(this double a, double b)
    {
        return Math.Abs(b - a) < 1e-11d * Math.Max(Math.Pow(10d, Math.Ceiling(Math.Log10(Math.Max(a, b)))), 1d);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Max(int a, int b) => a >= b  ? a : b;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Max(int a, int b, int c) => Max(c, Max(a, b));
    public static int Max(params int[] args)
    {
        int max = args[0];
        for (int i = 1; i < args.Length; i++)
        {
            max = Max(max, args[i]);
        }
        return max;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Min(int a, int b) => a <= b ? a : b;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Min(int a, int b, int c) => Min(c, Min(a, b));
    public static int Min(params int[] args)
    {
        int min = args[0];
        for (int i = 1; i < args.Length; i++)
        {
            min = Min(min, args[i]);
        }
        return min;
    }

    /// <summary>
    /// t € [0, 1]
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Lerp(in int a, in int b, float t) => (int)(a + (b - a) * t);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Lerp(float a, float b, float t) => a + (b - a) * t;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsOdd(this int number) => (number & 1) != 0;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEven(this int number) => (number & 1) == 0;

    public static Vector3[] GetVertices(in this Bounds bounds)
    {
        Vector3[] res;
        if (Mathf.Abs(bounds.extents.z) < Mathf.Epsilon)
        {
            res = new Vector3[4]
            {
                new Vector3(bounds.center.x - bounds.extents.x * 0.5f, bounds.center.y + bounds.extents.y * 0.5f),
                new Vector3(bounds.center.x + bounds.extents.x * 0.5f, bounds.center.y + bounds.extents.y * 0.5f),
                new Vector3(bounds.center.x - bounds.extents.x * 0.5f, bounds.center.y - bounds.extents.y * 0.5f),
                new Vector3(bounds.center.x + bounds.extents.x * 0.5f, bounds.center.y - bounds.extents.y * 0.5f)
            };
        }
        else
        {
            res = new Vector3[8]
            {
                new Vector3(bounds.center.x - bounds.extents.x * 0.5f, bounds.center.y + bounds.extents.y * 0.5f, bounds.center.z + bounds.extents.z * 0.5f),
                new Vector3(bounds.center.x + bounds.extents.x * 0.5f, bounds.center.y + bounds.extents.y * 0.5f, bounds.center.z + bounds.extents.z * 0.5f),
                new Vector3(bounds.center.x - bounds.extents.x * 0.5f, bounds.center.y + bounds.extents.y * 0.5f, bounds.center.z - bounds.extents.z * 0.5f),
                new Vector3(bounds.center.x + bounds.extents.x * 0.5f, bounds.center.y + bounds.extents.y * 0.5f, bounds.center.z - bounds.extents.z * 0.5f),
                new Vector3(bounds.center.x - bounds.extents.x * 0.5f, bounds.center.y - bounds.extents.y * 0.5f, bounds.center.z + bounds.extents.z * 0.5f),
                new Vector3(bounds.center.x + bounds.extents.x * 0.5f, bounds.center.y - bounds.extents.y * 0.5f, bounds.center.z + bounds.extents.z * 0.5f),
                new Vector3(bounds.center.x - bounds.extents.x * 0.5f, bounds.center.y - bounds.extents.y * 0.5f, bounds.center.z - bounds.extents.z * 0.5f),
                new Vector3(bounds.center.x + bounds.extents.x * 0.5f, bounds.center.y - bounds.extents.y * 0.5f, bounds.center.z - bounds.extents.z * 0.5f)
            };
        }
        return res;
    }

    public static bool Contain(in this Bounds b, in Bounds bounds)
    {
        Vector3[] vertices = bounds.GetVertices();
        foreach (Vector3 v in vertices)
        {
            if (!b.Contains(v))
                return false;
        }
        return true;
    }

    public enum Side { Up, Down, Right, Left }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPointInRectangle(in Vector2 center, in Vector2 size, in Vector2 point)
    {
        return point.x >= center.x - size.x * 0.5f && point.x <= center.x + size.x * 0.5f && point.y >= center.y - size.y * 0.5f && point.y <= center.y + size.y * 0.5f;
    }

    public static Side GetRectangleSide(in Vector2 center, in Vector2 size, in Vector2 point)
    {
        bool up = IsPointInRectangle(new Vector2(center.x, center.y + (size.y * 0.25f)), new Vector2(size.x, size.y * 0.5f), point);
        bool down = !up && IsPointInRectangle(new Vector2(center.x, center.y - (size.y * 0.25f)), new Vector2(size.x, size.y * 0.5f), point);

        if(up || down)
        {
            bool right = IsPointInRectangle(new Vector2(center.x + (size.x * 0.25f), center.y), new Vector2(size.x * 0.5f, size.y), point);
            float vPercent = Mathf.Abs(point.y - center.y) / size.y;
            float hPercent = Mathf.Abs(point.x - center.x) / size.x;
            return vPercent >= hPercent ? (up ? Side.Up : Side.Down) : (right ? Side.Right : Side.Left);
        }

        float angle1, angle2;
        Vector2 topRight = new Vector2(center.x + (size.x * 0.5f), center.y + (size.y * 0.5f));
        Vector2 topLeft = new Vector2(center.x - (size.x * 0.5f), center.y + (size.y * 0.5f));

        angle1 = AngleHori(topRight, point);
        angle2 = AngleHori(topLeft, point);
        if (angle1 >= Mathf.PI * 0.25f && angle2 <= Mathf.PI * 0.75f)
        {
            return Side.Up;
        }

        Vector2 botRight = new Vector2(center.x + (size.x * 0.5f), center.y - (size.y * 0.5f));

        angle2 = AngleHori(botRight, point);
        if ((angle1 <= Mathf.PI * 0.25f || angle1 >= 1.5f * Mathf.PI) && (angle2 <= Mathf.PI * 0.5f || angle2 >= Mathf.PI * 1.75f))
        {
            return Side.Right;
        }

        Vector2 botLeft = new Vector2(center.x - (size.x * 0.5f), center.y - (size.y * 0.5f));
        angle1 = AngleHori(botLeft, point);
        if (angle1 >= Mathf.PI * 1.25f && angle2 <= Mathf.PI * 1.75f)
        {
            return Side.Down;
        }

        angle2 = AngleHori(topLeft, point);
        if ((angle1 <= Mathf.PI * 1.25f && angle1 >= Mathf.PI * 0.5f) && angle2 >= Mathf.PI * 0.75f)
        {
            return Side.Left;
        }

        return Side.Up;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float NearestFromZero(float a, float b) => Mathf.Abs(a) < Mathf.Abs(b) ? a : b;
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float FarestFromZero(float a, float b) => Mathf.Abs(a) > Mathf.Abs(b) ? a : b;

    public static decimal Sqrt(in decimal x)
    {
        if (x < 0) throw new OverflowException("Cannot calculate square root from a negative number");

        decimal current = (decimal)Math.Sqrt((double)x), previous;
        do
        {
            previous = current;
            if (previous == 0.0M) return 0;
            current = (previous + x / previous) * 0.5m;
        }
        while (Math.Abs(previous - current) > 0.0M);
        return current;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal Abs(in decimal x) => x >= 0m ? x : -x;

    public static bool FindARoot(Func<float, float> f, Func<float, float> fPrime, out float root, int maxIter = 50, float accuracy = 1e-5f)
    {
        return FindARoot(f, fPrime, Random.Rand(-100f, 100f), out root, maxIter, accuracy);
    }

    public static bool FindARoot(Func<float, float> f, Func<float, float> fPrime, float x0, out float root, int maxIter = 50, float accuracy = 1e-5f)
    {
        int iter = 0;
        float xk = x0;
        while (Mathf.Abs(f(xk)) > accuracy && iter <= maxIter)
        {
            xk = xk - (f(xk) / fPrime(xk));
            iter++;
        }
        if (iter >= maxIter)
        {
            root = xk;
            return false;
        }
        root = xk;
        return true;
    }

    #endregion

    #region Array

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetRandom<T>(this T[] array) => array[Random.RandExclude(0, array.Length)];

    public static T[] Clone<T>(this T[] array) where T : ICloneable<T>
    {
        T[] res = new T[array.Length];
        for (int i = 0; i < res.Length; i++)
        {
            res[i] = array[i].Clone();
        }
        return res;
    }

    public static T[,] Clone<T>(this T[,] Array)
    {
        T[,] a = new T[Array.GetLength(0), Array.GetLength(1)];
        for (int l = 0; l < a.GetLength(0); l++)
        {
            for (int c = 0; c < a.GetLength(1); c++)
            {
                a[l, c] = Array[l, c];
            }
        }
        return a;
    }

    public static int IndexOf<T>(this T[] arr, T value, bool sortArray = false) where T : IComparable
    {
        if(sortArray)
        {
            int IndexOfRecur(T[] arr, int min, int max, in T value)
            {
                if(max - min <= 1)
                {
                    if(max == min)
                        return arr[min].CompareTo(value) == 0 ? min : -1;
                    return arr[min].CompareTo(value) == 0 ? min : (arr[max].CompareTo(value) == 0 ? max : -1);
                }
                int mid = (min + max) / 2;

                if (arr[mid].CompareTo(value) >= 1)
                {
                    return IndexOfRecur(arr, min, mid, value);
                }
                return IndexOfRecur(arr, mid, max, value);
            }

            return IndexOfRecur(arr, 0, arr.Length - 1, value);
        }

        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i].CompareTo(value) == 0)
                return i;
        }
        return -1;
    }

    public static T[] Merge<T>(this T[] arr, T[] other)
    {
        T[] res = new T[arr.Length + other.Length];
        for (int i = 0; i < arr.Length; i++)
        {
            res[i] = arr[i];
        }
        for (int i = arr.Length; i < res.Length; i++)
        {
            res[i] = other[i - arr.Length];
        }
        return res;
    }

    public static List<T> Merge<T>(this List<T> lst, List<T> other)
    {
        List<T> res = new List<T>();
        for (int i = 0; i < lst.Count; i++)
        {
            res.Add(lst[i]);
        }
        for (int i = 0; i < other.Count; i++)
        {
            res.Add(other[i]);
        }
        return res;
    }

    #region Show

    public static void Print<T>(this T[] tab, string begMessage = "", string endMessage = "")
    {
        Debug.Log(begMessage + tab.ToString<T>() + endMessage);
    }

    public static string ToString<T>(this T[] arr)
    {
        if (arr.Length <= 0)
            return "[]";

        StringBuilder sb = new StringBuilder("[ ");
        for (int l = 0; l < arr.Length; l++)
        {
            sb.Append(arr[l].ToString());
            sb.Append(", ");
        }
        sb.Remove(sb.Length - 2, 2);
        sb.Append(" ]");
        return sb.ToString();
    }

    public static void ShowArray<T>(this T[,] tab)
    {
        string text = "";
        for (int l = 0; l < tab.GetLength(0); l++)
        {
            text = "[ ";
            for (int c = 0; c < tab.GetLength(1); c++)
            {
                text += tab[l, c].ToString() + ", ";
            }
            text = text.Remove(text.Length - 2, 2);
            text += " ],";
            Debug.Log(text);
        }
    }

    #endregion

    #endregion

    #region Normalise Array

    /// <summary>
    /// Normalise tout les éléments de l'array pour obtenir des valeur entre 0f et 1f, ainse le min de array sera 0f, et le max 1f.
    /// </summary>
    /// <param name="array">The array to normalised</param>
    public static void NormaliseArray(this float[] array)
    {
        float min = float.MaxValue, max = float.MinValue;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] < min)
                min = array[i];
            if (array[i] > max)
                max = array[i];
        }
        float maxMinusMin = max - min;
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = (array[i] - min) / maxMinusMin;
        }
    }

    /// <summary>
    /// Change array like the sum of each element make 1f
    /// </summary>
    /// <param name="array">the array to change, each element must to be positive</param>
    public static void GetProbabilityArray(this float[] array)
    {
        float sum = 0f;
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] < 0f)
            {
                Debug.LogWarning("Array[" + i + "] must to be positive : " + array[i]);
                return;
            }
            sum += array[i];
        }
        for (int i = 0; i < array.Length; i++)
        {
            array[i] /= sum;
        }
    }

    #endregion

    #region List

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T GetRandom<T>(this List<T> lst) => lst[Random.RandExclude(0, lst.Count)];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Last<T>(this List<T> lst) => lst[lst.Count - 1];

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveLast<T>(this List<T> lst) => lst.RemoveAt(lst.Count - 1);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveBeg<T>(this List<T> lst) => lst.RemoveAt(0);

    public static List<T> Distinct<T>(this List<T> lst)
    {
        List<T> result = new List<T>();
        foreach (T item in lst)
        {
            if(!result.Contains(item))
                result.Add(item);
        }
        return result;
    }

    /// <summary>
    /// Retourne lst1 union lst2
    /// </summary>
    /// <param name="lst1">La première liste</param>
    /// <param name="lst2">La seconde liste</param>
    /// <param name="doublon">SI on autorise ou pas les doublons</param>
    /// <returns></returns>     
    public static List<T> Merge<T>(this List<T> lst1, in List<T> lst2, bool doublon = false)//pas de doublon par defaut
    {
        return Merge(lst1, lst2);
    }

    public static List<T> Merge<T>(in List<T> lst1, in List<T> lst2, bool doublon = false)//pas de doublon par defaut
    {
        List<T> result = new List<T>();
        foreach (T nb in lst1)
        {
            if (doublon || !result.Contains(nb))
                result.Add(nb);
        }
        foreach (T nb in lst2)
        {
            if (doublon || !result.Contains(nb))
                result.Add(nb);
        }
        return result;
    }

    public static void Add<T>(this List<T> lst1, in List<T> lstToAdd, bool doublon = false)//pas de doublon par defaut
    {
        if (doublon)
        {
            foreach (T element in lstToAdd)
            {
                lst1.Add(element);
            }
        }
        else
        {
            foreach (T element in lstToAdd)
            {
                if (lst1.Contains(element))
                {
                    continue;
                }
                lst1.Add(element);
            }
        }

    }

    public static List<T> DeepClone<T>(this List<T> lst) where T : ICloneable<T>
    {
        List<T> res = new List<T>();
        foreach (T item in lst)
        {
            res.Add(item.Clone());
        }
        return res;
    }

    #endregion

    #region Opti

    public static bool IsOverUI(in Vector3 position)
    {
        PointerEventData eventDataCurrentPos = new PointerEventData(EventSystem.current) { position = position };
        List<RaycastResult>  res = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPos, res);
        return res.Count > 0;
    }

    public static Vector2 GetWorldPositionCanvasElement(RectTransform elem, Camera camera)
    {
        RectTransformUtility.ScreenPointToWorldPointInRectangle(elem, elem.position, camera, out var res);
        return res;
    }

    public static void DestroyChildren(this Transform t)
    {
        foreach (Transform child in t)
            UnityEngine.Object.Destroy(child.gameObject);
    }

    public static void DestroyChildren(this GameObject t)
    {
        foreach (Transform child in t.transform)
            UnityEngine.Object.Destroy(child.gameObject);
    }

    #endregion

    #region Invoke

    #region Invoke simple

    public static void InvokeWaitAFrame(this MonoBehaviour script, string methodName)
    {
        script.StartCoroutine(InvokeWaitAFrameCorout(script, methodName));
    }

    private static IEnumerator InvokeWaitAFrameCorout(MonoBehaviour script, string methodName)
    {
        yield return null;
        script.Invoke(methodName, 0f);
    }

    public static void Invoke(this MonoBehaviour script, Action method, float delay)
    {
        script.StartCoroutine(InvokeCorout(method, delay));
    }

    private static IEnumerator InvokeCorout(Action method, float delay)
    {
        yield return new WaitForSeconds(delay);
        method.Invoke();
    }

    #endregion

    #region Invoke<T>

    public static void Invoke<T>(this MonoBehaviour script, Action<T> method, T param, float delay)
    {
        script.StartCoroutine(InvokeCorout(method, param, delay));
    }

    private static IEnumerator InvokeCorout<T>(Action<T> method, T param, float delay)
    {
        yield return new WaitForSeconds(delay);
        method.Invoke(param);
    }

    public static void Invoke<T1, T2>(this MonoBehaviour script, Action<T1, T2> method, T1 param1, T2 param2, float delay)
    {
        script.StartCoroutine(InvokeCorout(method, param1, param2, delay));
    }

    private static IEnumerator InvokeCorout<T1, T2>(Action<T1, T2> method, T1 param1, T2 param2, float delay)
    {
        yield return new WaitForSeconds(delay);
        method.Invoke(param1, param2);
    }

    public static void Invoke<T1, T2, T3>(this MonoBehaviour script, Action<T1, T2, T3> method, T1 param1, T2 param2, T3 param3, float delay)
    {
        script.StartCoroutine(InvokeCorout(method, param1, param2, param3, delay));
    }

    private static IEnumerator InvokeCorout<T1, T2, T3>(Action<T1, T2, T3> method, T1 param1, T2 param2, T3 param3, float delay)
    {
        yield return new WaitForSeconds(delay);
        method.Invoke(param1, param2, param3);
    }

    #endregion

    #region InvokeRepeating

    public static void InvokeRepeating(this MonoBehaviour script, Action method, float deltaTime)
    {
        script.StartCoroutine(InvokeRepeatingCorout(method, deltaTime, -1f));
    }

    public static void InvokeRepeating(this MonoBehaviour script, Action method, float deltaTime, float duration)
    {
        script.StartCoroutine(InvokeRepeatingCorout(method, deltaTime, duration));
    }

    private static IEnumerator InvokeRepeatingCorout(Action method, float deltaTime, float duration)
    {
        float timeBeg = Time.time;
        float time = Time.time;

        while(duration < Mathf.Epsilon || Time.time - timeBeg < duration)
        {
            if(Time.time - time < deltaTime)
            {
                method.Invoke();
                time = Time.time;
            }
            yield return null;
        }
    }

    #endregion

    #region InvokeRepeating<T>

    public static void InvokeRepeating<T>(this MonoBehaviour script, Action<T> method, T param, float deltaTime)
    {
        script.StartCoroutine(InvokeRepeatingCorout(method, param, deltaTime, -1f));
    }

    public static void InvokeRepeating<T>(this MonoBehaviour script, Action<T> method, T param, float deltaTime, float duration)
    {
        script.StartCoroutine(InvokeRepeatingCorout(method, param, deltaTime, duration));
    }

    private static IEnumerator InvokeRepeatingCorout<T>(Action<T> method, T param, float deltaTime, float duration)
    {
        float timeBeg = Time.time;
        float time = Time.time;

        while (duration < Mathf.Epsilon || Time.time - timeBeg < duration)
        {
            if (Time.time - time < deltaTime)
            {
                method.Invoke(param);
                time = Time.time;
            }
            yield return null;
        }
    }

    #endregion

    #endregion

    #region Unity

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Dictionary<TKey, TValue> Clone<TKey, TValue>(this Dictionary<TKey, TValue> dict) => new Dictionary<TKey, TValue>(dict);

    public static Dictionary<TKey, TValue> DeepClone<TKey, TValue>(this Dictionary<TKey, TValue> dict) where TValue : ICloneable<TValue>
    {
        Dictionary<TKey, TValue> res = new Dictionary<TKey, TValue>();
        foreach(KeyValuePair<TKey, TValue> kvp in dict)
        {
            res.Add(kvp.Key, kvp.Value.Clone());
        }
        return res;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static List<T> Clone<T>(this List<T> lst) => new List<T>(lst);

    public static void AddToDontDestroyOnLoad(this MonoBehaviour obj)
    {
        obj.transform.parent = null;
        UnityEngine.Object.DontDestroyOnLoad(obj.gameObject);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RemoveFromDontDestroyOnLoad(this GameObject obj)
    {
        SceneManager.MoveGameObjectToScene(obj, SceneManager.GetActiveScene());
    }

    public static void GizmoDrawVector(in Vector2 origin, in Vector2 dir) => GizmoDrawVector(origin, dir, 1f, 0.39269908169f, Gizmos.DrawLine); // 0.39269908169f = 22.5° = pi/8 rad
    public static void GizmoDrawVector(in Vector2 origin, in Vector2 dir, Action<Vector3, Vector3> drawLineFunction) => GizmoDrawVector(origin, dir, 1f, 0.39269908169f, drawLineFunction);
    public static void GizmoDrawVector(in Vector2 origin, in Vector2 dir, float length) => GizmoDrawVector(origin, dir, length, 0.39269908169f, Gizmos.DrawLine);
    public static void GizmoDrawVector(in Vector2 origin, in Vector2 dir, float length, Action<Vector3, Vector3> drawLineFunction) => GizmoDrawVector(origin, dir, length, 0.39269908169f, drawLineFunction);
    public static void GizmoDrawVector(in Vector2 origin, in Vector2 dir, float length, float arrowAngle, Action<Vector3, Vector3> drawLineFunction)
    {
        Vector2 end = origin + dir * length;
        drawLineFunction(origin, end);
        float teta = AngleHori(origin, end);
        float a = Mathf.PI + teta + arrowAngle;
        drawLineFunction(end, end + new Vector2(length * 0.33f * Mathf.Cos(a), length * 0.33f * Mathf.Sin(a)));
        a = 2f * Mathf.PI - (Mathf.PI - teta) - arrowAngle;
        drawLineFunction(end, end + new Vector2(length * 0.33f * Mathf.Cos(a), length * 0.33f * Mathf.Sin(a)));
    }

    public static AnimationClip[] GetAnimationsClips(this Animator animator) => animator.runtimeAnimatorController.animationClips;

    public static string[] GetAnimations(this Animator animator)
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        string[] res = new string[clips.Length];
        for (int i = 0; i < clips.Length; i++)
        {
            res[i] = clips[i].name;
        }
        return res;
    }

    public static bool GetAnimationLength(this Animator anim, string name, out float length)
    {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        for (int i = 0; i < clips.Length; i++)
        {
            if (clips[i].name == name)
            {
                length = clips[i].length;
                return true;
            }
        }
        length = 0f;
        return false;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contain(this LayerMask layerMask, LayerMask layer) => layerMask.Contain((int)layer);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Contain(this LayerMask layerMask, int layer) => (layerMask & (1 << layer)) != 0;

    #endregion
}

#endregion