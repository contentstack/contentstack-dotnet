# Skills – Contentstack .NET SDK

Source of truth for detailed guidance. Read **[AGENTS.md](../AGENTS.md)** first, then open the skill that matches your task.

## When to use which skill

| Skill folder | Use when |
|--------------|----------|
| `dev-workflow` | Building the solution, versioning, CI workflows, onboarding, PR prep. |
| `sdk-core-patterns` | Architecture, `ContentstackClient`, HTTP layer, DI, plugins, request flow. |
| `query-building` | Query operators, fluent API, pagination, sync, taxonomy, `Query.cs`. |
| `models-and-serialization` | Entry/Asset models, JSON converters, `ContentstackCollection`, serialization. |
| `error-handling` | Exceptions, `ErrorMessages`, parsing API errors. |
| `testing` | Writing or debugging unit/integration tests, coverage, test layout. |
| `code-review` | Reviewing a PR against SDK-specific checklist. |

Each folder contains **`SKILL.md`** with YAML frontmatter (`name`, `description`) for agent discovery. Long-form content is under **`references/`** when applicable—open `SKILL.md` first, then follow links.

### Cursor

In Cursor, you can also reference a skill in chat with `@skills/<folder-name>` (for example `@skills/testing`).
