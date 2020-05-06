﻿using AcBlog.Client.WASM.Interops;
using AcBlog.Client.WASM.Models;
using AcBlog.SDK;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;

namespace AcBlog.Client.WASM.Shared
{
    public class BasePage : ComponentBase, IDisposable
    {
        private string _title;

        protected string BaseUri { get; set; }

        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected IBlogService Service { get; set; }

        [Inject]
        protected BlogSettings BlogSettings { get; set; }

        protected virtual string Title
        {
            get => _title; set
            {
                if (string.IsNullOrEmpty(value))
                    value = BlogSettings.Name;
                else
                    value = $"{value} - {BlogSettings.Name}";
                if (value != _title)
                {
                    _title = value;
                    StateHasChanged();
                }
            }
        }

        private string LocalAnchorJump { get; set; } = "";

        private string GetBaseUri()
        {
            var url = NavigationManager.Uri;
            var ind = url.IndexOf('#');
            if (ind >= 0)
                url = url.Remove(ind);
            return url;
        }

        protected override void OnInitialized()
        {
            BaseUri = GetBaseUri();
            Title = "";
            NavigationManager.LocationChanged += LocationChanged;
            LocationChanged(null, null);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await WindowInterop.SetTitle(JSRuntime, Title);
            if (!string.IsNullOrEmpty(LocalAnchorJump))
            {
                await WindowInterop.ScrollTo(JSRuntime, LocalAnchorJump);
                LocalAnchorJump = null;
            }
        }

        private void LocationChanged(object sender, LocationChangedEventArgs args)
        {
            var url = NavigationManager.Uri;
            if (url.StartsWith(BaseUri))
            {
                var frag = url[BaseUri.Length..];
                if (frag.StartsWith("#"))
                {
                    LocalAnchorJump = HttpUtility.UrlDecode(frag[1..]);
                    StateHasChanged();
                }
            }
        }

        public virtual void Dispose()
        {
            NavigationManager.LocationChanged -= LocationChanged;
        }
    }
}