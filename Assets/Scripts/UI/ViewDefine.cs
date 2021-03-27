using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DemoProj
{
    public class ViewDefine
    {
        public enum View
        {
            None,
            Home,
        }

        public enum Scene
        {
            Title,
            Home,
        }

        public class ViewData
        {
            public View view;
            public Scene scene;

            public ViewData(View view, Scene scene)
            {
                this.view = view;
                this.scene = scene;
            }
        }

        public static Dictionary<View, ViewData> viewDict = new Dictionary<View, ViewData>()
        {
            { View.Home,new ViewData(View.Home, Scene.Home)},
        };
    }
}
