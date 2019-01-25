using System;
using System.Collections.Generic;
using System.Linq;

namespace ColladaSharp.Models
{
    /// <summary>
    /// Describes a weighted group of up to 4 bones. Contains no actual transformation information.
    /// </summary>
    public class InfluenceDef
    {
        public const int MaxWeightCount = 4;

        public InfluenceDef(string bone)
        {
            Weights[0] = new BoneWeight(bone);
            ++WeightCount;
        }
        public InfluenceDef(params BoneWeight[] weights) => SetWeights(weights);
        
        public BoneWeight[] Weights { get; private set; } = new BoneWeight[MaxWeightCount];
        public int WeightCount { get; set; } = 0;

        public void AddWeight(BoneWeight weight)
        {
            if (WeightCount == MaxWeightCount)
            {
                List<BoneWeight> weights = Weights.ToList();
                weights.Add(weight);
                Weights = Optimize(weights, out int wc);
                WeightCount = wc;
                return;
            }
            Weights[WeightCount++] = weight;
        }
        public void SetWeights(params BoneWeight[] weights)
        {
           SetWeights(weights.ToList());
        }
        public void SetWeights(List<BoneWeight> weights)
        {
            Weights = Optimize(weights, out int wc);
            WeightCount = wc;
        }
        public void Optimize()
        {
            SetWeights(Weights);
        }
        public void Normalize()
        {
            Normalize(Weights);
        }
        public static BoneWeight[] Optimize(List<BoneWeight> weights, out int weightCount)
        {
            BoneWeight[] optimized = new BoneWeight[MaxWeightCount];
            if (weights.Count > 4)
            {
                int[] toRemove = new int[weights.Count - MaxWeightCount];
                for (int i = 0; i < toRemove.Length; ++i)
                    for (int j = 0; j < weights.Count; ++j)
                        if (!toRemove.Contains(j + 1) &&
                            (toRemove[i] == 0 || weights[j].Weight < weights[toRemove[i] - 1].Weight))
                            toRemove[i] = j + 1;
                foreach (int k in toRemove)
                    weights.RemoveAt(k - 1);
            }
            weightCount = weights.Count;
            for (int i = 0; i < weights.Count; ++i)
                optimized[i] = weights[i];
            Normalize(optimized);
            return optimized;
        }
        /// <summary>
        /// Makes sure all weights add up to 1.0f.
        /// Does not modify any locked weights.
        /// </summary>
        public static void Normalize(IEnumerable<BoneWeight> weights, int weightDecimalPlaces = 7)
        {
            float denom = 0.0f;
            foreach (BoneWeight b in weights)
                if (b != null)
                    denom += b.Weight;
            
            if (denom > 0.0f)
                foreach (BoneWeight b in weights)
                    if (b != null)
                        b.Weight = (float)Math.Round(b.Weight / denom, weightDecimalPlaces);
        }
        public static bool operator ==(InfluenceDef left, InfluenceDef right)
        {
            if (left is null)
                return right is null;
            return left.Equals(right);
        }
        public static bool operator !=(InfluenceDef left, InfluenceDef right)
        {
            if (left is null)
                return !(right is null);
            return !left.Equals(right);
        }
        public override bool Equals(object obj)
        {
            InfluenceDef other = obj as InfluenceDef;
            if (other == null || WeightCount == other.WeightCount)
                return false;
            
            for (int i = 0; i < WeightCount; ++i)
                if (Weights[i] != other.Weights[i])
                    return false;

            return true;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
