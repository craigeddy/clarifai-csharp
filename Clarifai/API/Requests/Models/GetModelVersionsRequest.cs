﻿using System.Collections.Generic;
using Clarifai.DTOs.Models;
using Newtonsoft.Json.Linq;

namespace Clarifai.API.Requests.Models
{
    /// <summary>
    /// Request for retrieving all model-versions for a certain modelID.
    /// </summary>
    public class GetModelVersionsRequest : ClarifaiPaginatedRequest<List<ModelVersion>>
    {
        protected override RequestMethod Method => RequestMethod.GET;
        protected override string Url => $"/v2/models/{_modelID}/versions";

        private readonly string _modelID;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="client">the Clarifai client</param>
        /// <param name="modelID">the model ID</param>
        public GetModelVersionsRequest(IClarifaiClient client, string modelID) : base(client)
        {
            _modelID = modelID;
        }

        /// <inheritdoc />
        protected override JObject HttpRequestBody()
        {
            return new JObject();
        }

        /// <inheritdoc />
        protected override List<ModelVersion> Unmarshaller(dynamic jsonObject)
        {
            var result = new List<ModelVersion>();
            foreach (dynamic modelVersion in jsonObject.model_versions) {
                result.Add(ModelVersion.Deserialize(modelVersion));
            }
            return result;
        }
    }
}
