
using System;
using System.Collections.Generic;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

public partial class ESPNApiNFLSeasons
{
    [JsonPropertyName("count")]
    public long Count { get; set; }

    [JsonPropertyName("pageIndex")]
    public long PageIndex { get; set; }

    [JsonPropertyName("pageSize")]
    public long PageSize { get; set; }

    [JsonPropertyName("pageCount")]
    public long PageCount { get; set; }

    [JsonPropertyName("items")]
    public Item[] Items { get; set; }
}

public partial class Item
{
    [JsonPropertyName("$ref")]
    public Uri Ref { get; set; }
}
