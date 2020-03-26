﻿using System.Collections.Generic;
using Examine;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Core.Models;
using Umbraco.Core.PropertyEditors;
using Umbraco.Core.Services;
using Umbraco.Core.Strings;
using Umbraco.Examine;

namespace Umbraco.Web.Search
{

    /// <summary>
    /// Configures and installs Examine.
    /// </summary>
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public sealed class ExamineComposer : ComponentComposer<ExamineComponent>, ICoreComposer
    {
        public override void Compose(Composition composition)
        {
            base.Compose(composition);

            // populators are not a collection: one cannot remove ours, and can only add more
            // the container can inject IEnumerable<IIndexPopulator> and get them all
            composition.Register<MemberIndexPopulator>(Lifetime.Singleton);
            composition.Register<ContentIndexPopulator>(Lifetime.Singleton);
            composition.Register<PublishedContentIndexPopulator>(Lifetime.Singleton);
            composition.Register<MediaIndexPopulator>(Lifetime.Singleton);

            composition.Register<IndexRebuilder>(Lifetime.Singleton);
            composition.RegisterUnique<IUmbracoIndexConfig, UmbracoIndexConfig>();
            composition.RegisterUnique<IIndexDiagnosticsFactory, IndexDiagnosticsFactory>();            
            composition.RegisterUnique<IPublishedContentValueSetBuilder>(factory =>
                new ContentValueSetBuilder(
                    factory.GetInstance<PropertyEditorCollection>(),
                    factory.GetInstance<UrlSegmentProviderCollection>(),
                    factory.GetInstance<IUserService>(),
                    factory.GetInstance<IShortStringHelper>(),
                    true));
            composition.RegisterUnique<IContentValueSetBuilder>(factory =>
                new ContentValueSetBuilder(
                    factory.GetInstance<PropertyEditorCollection>(),
                    factory.GetInstance<UrlSegmentProviderCollection>(),
                    factory.GetInstance<IUserService>(),
                    factory.GetInstance<IShortStringHelper>(),
                    false));
            composition.RegisterUnique<IValueSetBuilder<IMedia>, MediaValueSetBuilder>();
            composition.RegisterUnique<IValueSetBuilder<IMember>, MemberValueSetBuilder>();
            composition.RegisterUnique<BackgroundIndexRebuilder>();
        }
    }
}