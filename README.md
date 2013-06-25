# DependencyResolver

Computes a list of reference assemblies (dependencies) for a given [`AssemblyName`][1] instance (dependency tree root).<br/>
The list contains all recursive assemblies, and is ordered from leaf to root, allowing users to load the assemlies in the order they are returned.

Assemblies in the global assembly cache (GAC) are omitted from the result list.


## Usage

See the [DependencyResolver.Test][2] project for some code samples.

Don't forget to edit your config file like [so][3].


  [1]: http://msdn.microsoft.com/en-us/library/system.reflection.assemblyname.aspx
  [2]: DependencyResolver.Test
  [3]: DependencyResolver/App.config.sample
