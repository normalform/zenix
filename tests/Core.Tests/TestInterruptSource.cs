// Copyright (c) 2025 Zenix Project
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Zenix.Core.Interrupt;

namespace Zenix.Tests.Core.Tests;

/// <summary>
/// Represents a test or mock interrupt source for unit testing
/// </summary>
public sealed class TestInterruptSource : InterruptSourceBase
{
    /// <summary>
    /// Creates a new test interrupt source
    /// </summary>
    /// <param name="testId">Unique test identifier</param>
    /// <param name="description">Test description</param>
    public TestInterruptSource(string testId, string? description = null)
    {
        Id = $"TEST_{testId}";
        Name = $"Test {testId}";
        Description = description ?? Name;
    }

    /// <summary>
    /// Gets the unique identifier for this interrupt source
    /// </summary>
    public override string Id { get; }

    /// <summary>
    /// Gets a human-readable name for this interrupt source
    /// </summary>
    public override string Name { get; }

    /// <summary>
    /// Gets the description of this interrupt source
    /// </summary>
    public override string Description { get; }

    /// <summary>
    /// Gets the category of this interrupt source
    /// </summary>
    public override InterruptSourceCategory Category => InterruptSourceCategory.Test;
}
