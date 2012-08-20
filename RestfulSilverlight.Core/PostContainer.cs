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
    public class PostContainer
    {
        private object _route;

        public PostContainer(object route)
        {
            _route = route;
        }

        private object _callback;
        public PostContainer WhenFinished(Action action)
        {
            CompleteCallback callback =
               () =>
               {
                   action();
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

        public void Go()
        {
            RestService restService = new RestService();
            restService.Post(_route, _callback as CompleteCallback);
        }
    }
}
