﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Health.Fhir.Core.Exceptions;
using static Hl7.Fhir.Model.Bundle;

namespace Microsoft.Health.Fhir.Api.Features.Resources.Bundle
{
    public static class TransactionValidator
    {
        public static void ValidateTransaction(HashSet<string> resourceIdList, EntryComponent entry)
        {
            if (ValidateBundleEntry(entry))
            {
                string resourceId = GetResourceUrl(entry);

                if (!string.IsNullOrEmpty(resourceId))
                {
                    if (resourceIdList.Contains(resourceId))
                    {
                        throw new RequestNotValidException(string.Format(Api.Resources.ResourcesMustBeUnique, entry.Request.Url));
                    }

                    resourceIdList.Add(resourceId);
                }
            }
        }

        private static bool ValidateBundleEntry(EntryComponent entry)
        {
            string requestUrl = entry.Request.Url;

            // Check for duplicate resources within a bundle entry is skipped if the entry is bundle or if the request within a entry is not modifying the resource.
            return !(entry.Resource.ResourceType == Hl7.Fhir.Model.ResourceType.Bundle
                || entry.Request.Method == HTTPVerb.GET
                || (entry.Request.Method == HTTPVerb.POST && requestUrl.Contains("_search", StringComparison.InvariantCulture))
                || requestUrl.Contains("$", StringComparison.InvariantCulture));
        }

        private static string GetResourceUrl(EntryComponent component)
        {
            if (component.Request.Method == HTTPVerb.POST)
            {
                return component.FullUrl;
            }

            return component.Request.Url;
        }
    }
}
