---
name: models-and-serialization
description: Model and serialization patterns for the Contentstack .NET SDK — Entry/Asset shape, catch-all Object dictionary, generic Fetch<T>/Find<T> projections, CSJsonConverter attribute-driven converter registration, EntryJsonConverter/AssetJsonConverter, ContentstackCollection<T>, JsonProperty for API name mismatches, entry variants. Use when adding new model types, writing JSON converters, working with entry variants, embedded assets, or modifying serialization settings.
---

# Models and Serialization

## When to use

- New models, converters, or `JsonProperty` mappings.
- Entry variants, embedded references, RTE/modular blocks behavior.
- Tuning `SerializerSettings`.

## Invariants

- CDA JSON is **snake_case** — use **`[JsonProperty("snake_case")]`** on mapped properties; do not rely on default PascalCase naming.
- Custom converters: **`[CSJsonConverter("Name")]`** on the model + **`JsonConverter`** implementation in **`Contentstack.Core/Internals/`**.
- New converters are registered automatically at client init (see reference).
- Use **`Newtonsoft.Json`** only — not `System.Text.Json`.

## Reference

**[references/serialization-patterns.md](references/serialization-patterns.md)** — consumer patterns (Entry/Asset, Fetch/Find, JsonProperty, variants, new models, SerializerSettings), converter internals, `parseJObject<T>`, RTE/modular blocks, collection edge cases.
