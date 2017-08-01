using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * An interface allowing nodes to support FileSystem operations.
 */
public interface IFileSystem {
	// Assert that classes implementing this have a FileSystem instance.

	FileSystem fileSystem { get; }
}
