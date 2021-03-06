﻿using System.Collections.Generic;

namespace Clarifai.DTOs.Predictions
{
    public class Focus : IPrediction
    {
        public string TYPE => "focus";

        public Crop Crop { get; }

        public decimal Density { get; }

        public decimal Value { get; }

        private Focus(Crop crop, decimal density, decimal value)
        {
            Crop = crop;
            Density = density;
            Value = value;
        }

        public static Focus Deserialize(dynamic jsonObject, decimal value)
        {
            return new Focus(
                DTOs.Crop.Deserialize(jsonObject.region_info.bounding_box),
                (decimal)jsonObject.data.focus.density,
                value);
        }

        public override bool Equals(object obj)
        {
            return obj is Focus focus &&
                   EqualityComparer<Crop>.Default.Equals(Crop, focus.Crop) &&
                   Density == focus.Density &&
                   Value == focus.Value;
        }

        public override int GetHashCode()
        {
            var hashCode = -1649476860;
            hashCode = hashCode * -1521134295 + EqualityComparer<Crop>.Default.GetHashCode(Crop);
            hashCode = hashCode * -1521134295 + Density.GetHashCode();
            hashCode = hashCode * -1521134295 + Value.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return $"[Focus: (crop: {Crop}, density: {Density}, value: {Value})]";
        }
    }
}
