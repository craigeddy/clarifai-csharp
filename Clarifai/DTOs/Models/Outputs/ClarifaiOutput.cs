﻿using System;
using System.Collections.Generic;
using System.Linq;
using Clarifai.DTOs.Inputs;
using Clarifai.DTOs.Predictions;
using Clarifai.Exceptions;
using Newtonsoft.Json.Linq;

namespace Clarifai.DTOs.Models.Outputs
{
    /// <summary>
    /// Output returned after running a request. Encapsulates the response content and other
    /// data the response returnes.
    /// </summary>
    public class ClarifaiOutput
    {
        /// <summary>
        /// The output ID.
        /// </summary>
        public string ID { get; }

        /// <summary>
        /// The output status.
        /// </summary>
        public ClarifaiStatus Status { get; }

        /// <summary>
        /// Date & time of output creation.
        /// </summary>
        public DateTime CreatedAt { get; }

        /// <summary>
        /// The input.
        /// </summary>
        public IClarifaiInput Input { get; }


        /// <summary>
        /// The data.
        /// </summary>
        public List<IPrediction> Data { get; }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="id">the output ID</param>
        /// <param name="status">the output status</param>
        /// <param name="createdAt">date & time of output creation</param>
        /// <param name="input">the input</param>
        /// <param name="data">the data</param>
        protected ClarifaiOutput(string id, ClarifaiStatus status, DateTime createdAt,
            IClarifaiInput input, List<IPrediction> data)
        {
            ID = id;
            Status = status;
            CreatedAt = createdAt;
            Input = input;
            Data = data;
        }

        /// <summary>
        /// Deserializes the object out of a JSON dynamic object.
        /// </summary>
        /// <param name="modelType">the model type</param>
        /// <param name="jsonObject">the JSON object</param>
        /// <returns>the deserialized object</returns>
        public static ClarifaiOutput Deserialize(ModelType modelType, dynamic jsonObject)
        {
            dynamic data = DeserializePredictions(modelType, jsonObject);

            return new ClarifaiOutput(
                (string)jsonObject.id,
                ClarifaiStatus.Deserialize(jsonObject.status),
                (DateTime) jsonObject.created_at,
                jsonObject.input != null ? ClarifaiInput.Deserialize(jsonObject.input) : null,
                data);
        }

        protected static List<IPrediction> DeserializePredictions(ModelType modelType,
            dynamic jsonObject)
        {
            var propertyValues = (JObject) jsonObject.data;

            var data = new List<IPrediction>();
            if (propertyValues.Count > 0)
            {
                string typeName = modelType.Prediction.Name;
                switch (typeName)
                {
                    case "Color":
                    {
                        foreach (dynamic color in jsonObject.data.colors)
                        {
                            data.Add(Color.Deserialize(color));
                        }
                        break;
                    }
                    case "Concept":
                    {
                        foreach (dynamic concept in jsonObject.data.concepts)
                        {
                            data.Add(Concept.Deserialize(concept));
                        }
                        break;
                    }
                    case "Demographics":
                    {
                        foreach (dynamic demographics in jsonObject.data.regions)
                        {
                            data.Add(Demographics.Deserialize(demographics));
                        }
                        break;
                    }
                    case "Embedding":
                    {
                        foreach (dynamic embedding in jsonObject.data.embeddings)
                        {
                            data.Add(Embedding.Deserialize(embedding));
                        }
                        break;
                    }
                    case "FaceConcepts":
                    {
                        foreach (dynamic faceConcepts in
                            jsonObject.data.regions)
                        {
                            data.Add(FaceConcepts.Deserialize(faceConcepts));
                        }
                        break;
                    }
                    case "FaceDetection":
                    {
                        foreach (dynamic faceDetection in jsonObject.data.regions)
                        {
                            data.Add(FaceDetection.Deserialize(faceDetection));
                        }
                        break;
                    }
                    case "FaceEmbedding":
                    {
                        foreach (dynamic faceEmbedding in jsonObject.data.regions)
                        {
                            data.Add(FaceEmbedding.Deserialize(faceEmbedding));
                        }
                        break;
                    }
                    case "Focus":
                    {
                        foreach (dynamic focus in jsonObject.data.regions)
                        {
                            data.Add(Focus.Deserialize(focus,
                                (decimal) jsonObject.data.focus.value));
                        }
                        break;
                    }
                    case "Frame":
                    {
                        foreach (dynamic frame in jsonObject.data.frames)
                        {
                            data.Add(Frame.Deserialize(frame));
                        }
                        break;
                    }
                    case "Logo":
                    {
                        foreach (dynamic logo in jsonObject.data.regions)
                        {
                            data.Add(Logo.Deserialize(logo));
                        }
                        break;
                    }
                    default:
                    {
                        throw new ClarifaiException(
                            string.Format("Unknown output type `{0}`", typeName));
                    }
                }
            }
            return data;
        }

        public override bool Equals(object obj)
        {
            return obj is ClarifaiOutput output &&
                   ID == output.ID &&
                   EqualityComparer<ClarifaiStatus>.Default.Equals(Status, output.Status) &&
                   CreatedAt == output.CreatedAt &&
                   EqualityComparer<IClarifaiInput>.Default.Equals(Input, output.Input) &&
                   EqualityComparer<List<IPrediction>>.Default.Equals(Data, output.Data);
        }

        public override int GetHashCode()
        {
            var hashCode = 218169885;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ID);
            hashCode = hashCode * -1521134295 +
                       EqualityComparer<ClarifaiStatus>.Default.GetHashCode(Status);
            hashCode = hashCode * -1521134295 + CreatedAt.GetHashCode();
            hashCode = hashCode * -1521134295 +
                       EqualityComparer<IClarifaiInput>.Default.GetHashCode(Input);
            hashCode = hashCode * -1521134295 +
                       EqualityComparer<List<IPrediction>>.Default.GetHashCode(Data);
            return hashCode;
        }

        public override string ToString()
        {
            return $"[ClarifaiOutput: (ID: {ID})]";
        }
    }


    /// <inheritdoc />
    /// <typeparam name="T">the data type</typeparam>
    public class ClarifaiOutput<T> : ClarifaiOutput where T : IPrediction
    {
        public new List<T> Data => base.Data.Cast<T>().ToList();

        /// <inheritdoc />
        private ClarifaiOutput(string id, ClarifaiStatus status, DateTime createdAt,
            IClarifaiInput input, List<IPrediction> rawData)
            : base(id, status, createdAt, input, rawData)
        { }

        /// <summary>
        /// Deserializes the object out of a JSON dynamic object.
        /// </summary>
        /// <param name="jsonObject">the JSON dynamic object of an output</param>
        /// <returns>the deserialized object</returns>
        public static ClarifaiOutput<T> Deserialize(dynamic jsonObject)
        {
            Type type = typeof(T);
            ModelType modelType = ModelType.ConstructFromName(type.Name);

            dynamic data = DeserializePredictions(modelType, jsonObject);
            return new ClarifaiOutput<T>(
                (string)jsonObject.id,
                ClarifaiStatus.Deserialize(jsonObject.status),
                (DateTime) jsonObject.created_at,
                jsonObject.input != null ? ClarifaiInput.Deserialize(jsonObject.input) : null,
                data);
        }
    }
}
