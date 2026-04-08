---
title: HttpResponseMessages
permalink: /httpresponsemessages/
layout: single
classes: wide
sidebar:
  nav: "sidebar"
---

Assertions on `HttpResponseMessage` were removed in Awesome Assertions 8.0. Use one of the following community packages instead:

- [FluentAssertions.Web](https://github.com/adrianiftode/FluentAssertions.Web) for asserts on a materialised `HttpResponseMessage`.
- [Fluent.Client.AwesomeAssertions](https://github.com/lepoco/fluent) for asserts directly on `Task<HttpResponseMessage>`; the task is awaited internally.

## Fluent.Client.AwesomeAssertions

```powershell
dotnet add package Fluent.Client.AwesomeAssertions
# optional — adds .Get() / .Post() / .Authorize() helpers on HttpClient
dotnet add package Fluent.Client
```

### Status codes

```csharp
await client.PostAsync("/api/users", content).Should().Succeed();
await client.PostAsync("/api/users", content).Should().SucceedWith(HttpStatusCode.Created);
await client.GetAsync("/api/users/999").Should().BeNotFound();
await client.DeleteAsync("/api/users/1").Should().HaveStatusCode(HttpStatusCode.NoContent);
```

Named shortcuts: `BeCreated()` · `BeNoContent()` · `BeBadRequest()` · `BeUnauthorized()` · `BeForbidden()` · `BeNotFound()` · `BeConflict()`

Use `FailWith(HttpStatusCode)` when you need to assert a specific non-2xx code without a named shortcut. Use `HaveStatusCode` when the code could be either success or failure.

### Headers

```csharp
await client.GetAsync("/api/users").Should().HaveHeader("X-Request-Id");
await client.GetAsync("/api/users").Should().HaveHeaderWithValue("Cache-Control", "no-cache");
await client.GetAsync("/api/users").Should().HaveContentType("application/json");
```

### Body

`Satisfy<T>` deserialises the JSON body regardless of status code — useful for asserting `ProblemDetails` on error responses:

```csharp
await client.GetAsync("/api/users/1").Should().Satisfy<User>(u => u.Id.Should().Be(1));

await client.PostAsync("/api/users", bad).Should().Satisfy<ProblemDetails>(p => p.Status.Should().Be(400));
```

`SucceedWith<T>` combines the 2xx check and body inspection in one call and short-circuits before deserialising on failure:

```csharp
await client.PostAsync("/api/users", content).Should().SucceedWith<User>(u => u.Name.Should().Be("John"));
```

Pass a raw `Action<HttpResponseMessage>` (or its async equivalent) to `Satisfy` for full control:

```csharp
await client.GetAsync("/api/data").Should().Satisfy(r => r.Headers.ETag.Should().NotBeNull());
```

### Customising JSON deserialisation

Replace `HttpResponseMessageTaskAssertions.DefaultJsonOptions` once at test-suite start-up to apply custom converters globally. The built-in defaults are case-insensitive property matching, trailing commas allowed, and enums as strings.

### Integration testing

```csharp
await client.PutAsync($"v1/orders/{id}", body).Should().BeCreated();
await client.GetAsync($"v1/orders/{id}").Should().Satisfy<Order>(o => o.Status.Should().Be("Pending"));
await client.PutAsync($"v1/orders/{id}/confirm", null).Should().Succeed();
```

See the [Fluent.Client.AwesomeAssertions README](https://github.com/lepoco/fluent/blob/main/src/Fluent.Client.AwesomeAssertions/README.md) for the full API reference and more examples.

---

## FluentAssertions.Web

```powershell
dotnet add package FluentAssertions.Web
```

It provides assertions specific to HTTP responses and outputs rich errors messages when the tests fail, so less time with debugging is spent.

```csharp
[Fact]
public async Task Post_ReturnsOk()
{
    // Arrange
    var client = _factory.CreateClient();

    // Act
    var response = await client.PostAsync("/api/comments", new StringContent(
    """
    {
      "author": "John",
      "content": "Hey, you..."
    }
    """, Encoding.UTF8, "application/json"));

    // Assert
    response.Should().Be200Ok();
}
```
