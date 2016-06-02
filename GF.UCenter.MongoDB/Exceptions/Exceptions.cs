using System;

namespace GF.UCenter.MongoDB
{
    public class ModelNotFoundException : Exception
    {

    }

    public class DatabaseException : Exception
    {
        public DatabaseException(string message)
            :base($"The operation failed with {message}")
        {

        }
    }

    public class ModelLockedException : Exception
    {
        public ModelLockedException(string type, Guid id)
            :base($"Model {type} locked ({id})")
        {

        }
    }

    public class InvalidVersionException : Exception
    {
        public InvalidVersionException(string type, Guid id, int currentVersion, int eventVersion)
            :base($"Updating {type} failed ({id}), current version = {currentVersion}, event version = {eventVersion}")
        {

        }
    }

    public class AggregateDeletedException : Exception
    {

    }

    public class ConcurrencyException : Exception
    {

    }
}
