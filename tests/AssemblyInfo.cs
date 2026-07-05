// The testing assembly uses functionality we
// know as the 'unsafe' context. Pointers are apparently
// too terrifying for the CLS. No compliance then.
[assembly: CLSCompliant(false)]

// MSTest requires I explicitly enable or disable test parallelization.
// Method-level breaks native RSML testing.
[assembly: Parallelize(Scope = ExecutionScope.ClassLevel)]
