# SDK-specific red flags

Quick scan for anti-patterns in PRs:

```
❌ new HttpClient()               — use HttpRequestHandler
❌ throw new Exception("message") — use typed ContentstackException subclass
❌ "hardcoded_field_name"         — use [JsonProperty] or ErrorMessages constant
❌ public Config GetConfig()      — Config is internal by design
❌ return void                    — Query methods return Query (fluent)
❌ [JsonProperty] omitted         — CDA uses snake_case; PascalCase won't deserialize
❌ <Version> in .csproj           — use Directory.Build.props
```

For full category-by-category review, use [code-review-checklist.md](code-review-checklist.md).
