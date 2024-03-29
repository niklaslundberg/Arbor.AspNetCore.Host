﻿using System;
using System.IO;
using System.Linq;
using Arbor.App.Extensions.Application;
using Arbor.App.Extensions.ExtensionMethods;
using Arbor.KVConfiguration.JsonConfiguration;
using Arbor.KVConfiguration.Urns;
using JetBrains.Annotations;

namespace Arbor.AspNetCore.Host.Configuration
{
    [UsedImplicitly]
    public sealed class UserConfigUpdater : IDisposable
    {
        private readonly ConfigurationInstanceHolder _configurationHolder;
        private readonly string _fileName;
        private FileSystemWatcher? _fileSystemWatcher;
        private bool _isDisposed;

        public UserConfigUpdater(ConfigurationInstanceHolder configurationHolder,
            EnvironmentConfiguration applicationEnvironment)
        {
            _configurationHolder = configurationHolder;

            _fileName = Path.Combine(applicationEnvironment.ContentBasePath ?? Directory.GetCurrentDirectory(),
                "config.user");

            if (File.Exists(_fileName))
            {
                var fileInfo = new FileInfo(_fileName);

                if (fileInfo.Directory is { })
                {
                    _fileSystemWatcher = new FileSystemWatcher(fileInfo.Directory.FullName, fileInfo.Name);
                    _fileSystemWatcher.Changed += WatcherOnChanged;
                    _fileSystemWatcher.Created += WatcherOnChanged;
                    _fileSystemWatcher.Renamed += WatcherOnChanged;
                }
            }
        }

        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            if (_fileSystemWatcher is { })
            {
                _fileSystemWatcher.EnableRaisingEvents = false;
                _fileSystemWatcher.Changed -= WatcherOnChanged;
                _fileSystemWatcher.Created -= WatcherOnChanged;
                _fileSystemWatcher.Renamed -= WatcherOnChanged;
                _fileSystemWatcher.Dispose();
            }

            _fileSystemWatcher = null;
            _isDisposed = true;
        }

        private void WatcherOnChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            if (!File.Exists(_fileName))
            {
                return;
            }

            var types = _configurationHolder.RegisteredTypes.Where(type => type.HasAttribute<UrnAttribute>()).ToArray();

            var jsonKeyValueConfiguration = new JsonKeyValueConfiguration(_fileName);

            foreach (var type in types)
            {
                var allInstances = jsonKeyValueConfiguration.GetNamedInstances(type);

                foreach (var instance in allInstances)
                {
                    if (instance is { })
                    {
                        _configurationHolder.Add(instance);
                    }
                }
            }
        }

        public void Start()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(UserConfigUpdater));
            }

            if (File.Exists(_fileName) && _fileSystemWatcher is { })
            {
                _fileSystemWatcher.EnableRaisingEvents = true;
            }
        }
    }
}