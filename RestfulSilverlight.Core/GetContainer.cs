using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace RestfulSilverlight.Core
{
    public class GetContainer<T> where T : class, new()
    {
        private object _route;

        public GetContainer(object route)
        {
            _route = route;
        }

        private object _callback;
        public GetContainer<T> WhenFinished(Action<T> action)
        {
            CompleteCallback<T> callback =
               (t) =>
               {
                   action(t);
                   if (_nextContainer != null)
                   {
                       _nextContainer.Go();
                   }
               };

            _callback = callback;
            return this;
        }

        private dynamic _nextContainer;
        public GetContainer<T> ThenGet<T>(object route) where T : class, new()
        {
            _nextContainer = new GetContainer<T>(route);
            return _nextContainer;
        }

        public GetContainer<T> ThenGet<T>() where T : class, new()
        {
            _nextContainer = new GetContainer<T>(null);
            return _nextContainer;
        }

        public PostContainer ThenPost(object route)
        {
            _nextContainer = new PostContainer(route);
            return _nextContainer;
        }

        public PostContainer ThenPost()
        {
            _nextContainer = new PostContainer(null);
            return _nextContainer;
        }

        private Func<object> _deferredRouteRetrieval;
        public GetContainer<T> ForRoute(Func<object> deferredRouteRetrieval)
        {
            _deferredRouteRetrieval = deferredRouteRetrieval;
            return this;
        }

        public void Go()
        {
            if (_deferredRouteRetrieval != null)
            {
                _route = _deferredRouteRetrieval();
            }

            RestService restService = new RestService();
            restService.Get<T>(_route, _callback as CompleteCallback<T>);
        }
    }
}
