using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FileSystemNS;

/// <summary>
/// An interface allowing nodes to support FileSystem operations.
/// </summary>
public interface IFileSystem {
    // Assert that classes implementing this have a FileSystem instance.

    FileSystem fileSystem { get; }
}
