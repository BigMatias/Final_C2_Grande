using System.Collections.Generic;
using UnityEngine;

public static class HeadlessMeshCache
{
    private static readonly Dictionary<Mesh, Mesh> Cache = new Dictionary<Mesh, Mesh>();

    public static Mesh Get(SkinnedMeshRenderer renderer, Transform headBone, float threshold)
    {
        Mesh source = renderer.sharedMesh;

        if (Cache.TryGetValue(source, out Mesh cached))
            return cached;

        Mesh headless = Build(source, renderer.bones, headBone, threshold);
        Cache[source] = headless;
        return headless;
    }

    private static Mesh Build(Mesh source, Transform[] bones, Transform headBone, float threshold)
    {
        bool[] isHeadBone = new bool[bones.Length];

        for (int i = 0; i < bones.Length; i++)
            isHeadBone[i] = bones[i] != null && bones[i].IsChildOf(headBone);

        BoneWeight[] weights = source.boneWeights;
        bool[] removeVertex = new bool[weights.Length];

        for (int v = 0; v < weights.Length; v++)
        {
            BoneWeight weight = weights[v];
            float headWeight = 0f;

            if (isHeadBone[weight.boneIndex0]) headWeight += weight.weight0;
            if (isHeadBone[weight.boneIndex1]) headWeight += weight.weight1;
            if (isHeadBone[weight.boneIndex2]) headWeight += weight.weight2;
            if (isHeadBone[weight.boneIndex3]) headWeight += weight.weight3;

            removeVertex[v] = headWeight >= threshold;
        }

        Mesh result = Object.Instantiate(source);
        result.name = source.name + "_Headless";

        List<int> keptTriangles = new List<int>();

        for (int subMesh = 0; subMesh < source.subMeshCount; subMesh++)
        {
            int[] triangles = source.GetTriangles(subMesh);
            keptTriangles.Clear();

            for (int t = 0; t < triangles.Length; t += 3)
            {
                int a = triangles[t];
                int b = triangles[t + 1];
                int c = triangles[t + 2];

                if (removeVertex[a] || removeVertex[b] || removeVertex[c])
                    continue;

                keptTriangles.Add(a);
                keptTriangles.Add(b);
                keptTriangles.Add(c);
            }

            result.SetTriangles(keptTriangles, subMesh);
        }

        return result;
    }
}