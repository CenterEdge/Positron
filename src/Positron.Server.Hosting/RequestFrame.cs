using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace Positron.Server.Hosting
{
    abstract class RequestFrame : IFeatureCollection
    {
        private Dictionary<Type, object> _extraFeatures;

        public IHttpRequestFeature Request { get; }
        public PositronResponse Response { get; }

        protected RequestFrame(IHttpRequestFeature requestContext)
        {
            Request = requestContext;
            Response = new PositronResponse();
        }

        public abstract Task ProcessRequestAsync();

        protected void InitializeHeaders()
        {
            if (Request.Headers == null)
            {
                Request.Headers = new HeaderDictionary();
            }
            if (Response.Headers == null)
            {
                Response.Headers = new HeaderDictionary();
            }
        }

        protected void InitializeStreams()
        {
            if (Response.Body == null)
            {
                Response.Body = new MemoryStream();
            }
        }

        #region Features

        private IEnumerable<KeyValuePair<Type, object>> GetFeatures()
        {
            yield return new KeyValuePair<Type, object>(typeof(IHttpRequestFeature), Request);
            yield return new KeyValuePair<Type, object>(typeof(IHttpResponseFeature), Response);

            if (_extraFeatures != null)
            {
                foreach (var pair in _extraFeatures)
                {
                    yield return pair;
                }
            }
        }

        public IEnumerator<KeyValuePair<Type, object>> GetEnumerator()
        {
            return GetFeatures().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public TFeature Get<TFeature>()
        {
            return (TFeature) this[typeof(TFeature)];
        }

        public void Set<TFeature>(TFeature instance)
        {
            this[typeof(TFeature)] = instance;
        }

        public bool IsReadOnly => false;
        public int Revision { get; private set; }

        public object this[Type key]
        {
            get
            {
                if (key == typeof(IHttpRequestFeature))
                {
                    return Request;
                }
                if (key == typeof(IHttpResponseFeature))
                {
                    return Response;
                }

                if (_extraFeatures != null)
                {
                    object extra;
                    if (_extraFeatures.TryGetValue(key, out extra))
                    {
                        return extra;
                    }
                }

                return null;
            }
            set
            {
                if (key == typeof(IHttpRequestFeature))
                {
                    throw new NotImplementedException();
                }
                if (key == typeof(IHttpResponseFeature))
                {
                    throw new NotImplementedException();
                }

                if (_extraFeatures == null)
                {
                    _extraFeatures = new Dictionary<Type, object>(2);
                }

                _extraFeatures[key] = value;
            }
        }

        #endregion
    }
}
