using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using OTA.Data.Entity.Models;

namespace OTA.Data
{
    /// <summary>
    /// Generic OTA group information
    /// </summary>
    public class Group
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public bool ApplyToGuests { get; set; }

        public string Parent { get; set; }

        public byte Chat_Red { get; set; }

        public byte Chat_Green { get; set; }

        public byte Chat_Blue { get; set; }

        public string Chat_Prefix { get; set; }

        public string Chat_Suffix { get; set; }
    }

//    /// <summary>
//    /// Permission node.
//    /// </summary>
//    public struct PermissionNode
//    {
//        public string Node { get; set; }
//
//        public bool Deny { get; set; }
//    }

    /// <summary>
    /// The interface behind custom permissions handlers
    /// </summary>
    public interface IPermissionHandler
    {
        Permission IsPermitted(string node, BasePlayer player);

        #region "Management"

        /// <summary>
        /// Find a group by name
        /// </summary>
        /// <returns>The group.</returns>
        Group FindGroup(string name);

        /// <summary>
        /// Add the or update a group.
        /// </summary>
        /// <returns><c>true</c>, if or update group was added, <c>false</c> otherwise.</returns>
        bool AddOrUpdateGroup(string name, bool applyToGuests = false, string parent = null, byte r = 255, byte g = 255, byte b = 255, string prefix = null, string suffix = null);

        /// <summary>
        /// Remove a group from the data store.
        /// </summary>
        /// <returns><c>true</c>, if the group was removed, <c>false</c> otherwise.</returns>
        /// <param name="name">Name.</param>
        bool RemoveGroup(string name);

        /// <summary>
        /// Add a group node to the data store
        /// </summary>
        /// <returns><c>true</c>, if the group node was added, <c>false</c> otherwise.</returns>
        bool AddGroupNode(string groupName, string node, bool deny = false);

        /// <summary>
        /// Remove a group node from the data store
        /// </summary>
        /// <returns><c>true</c>, if the group node was removed, <c>false</c> otherwise.</returns>
        bool RemoveGroupNode(string groupName, string node, bool deny = false);

        /// <summary>
        /// Fetches the list of group names from the data store.
        /// </summary>
        string[] GroupList();

        /// <summary>
        /// Fetch the list of nodes for a group
        /// </summary>
        PermissionNode[] GroupNodes(string groupName);

        /// <summary>
        /// Add a user to a group.
        /// </summary>
        /// <returns><c>true</c>, if the user was added to the group, <c>false</c> otherwise.</returns>
        bool AddUserToGroup(string username, string groupName);

        /// <summary>
        /// Remove a user from a group
        /// </summary>
        /// <returns><c>true</c>, if the user was removed from the group, <c>false</c> otherwise.</returns>
        bool RemoveUserFromGroup(string username, string groupName);

        /// <summary>
        /// Add a node to the user.
        /// </summary>
        /// <returns><c>true</c>, if the node was added to the user, <c>false</c> otherwise.</returns>
        bool AddNodeToUser(string username, string node, bool deny = false);

        /// <summary>
        /// Removed a node from a user
        /// </summary>
        /// <returns><c>true</c>, if the node was removed from the user, <c>false</c> otherwise.</returns>
        bool RemoveNodeFromUser(string username, string node, bool deny = false);

        /// <summary>
        /// Fetch the group names a user is associated to
        /// </summary>
        /// <remarks>Currently should always be 1</remarks>
        string[] UserGroupList(string username);

        /// <summary>
        /// Fetch the nodes a user has specific access to
        /// </summary>
        /// <returns>The nodes.</returns>
        /// <param name="username">Username.</param>
        PermissionNode[] UserNodes(string username);

        /// <summary>
        /// Fetches the lowest inherited group
        /// </summary>
        /// <returns>The inherited group for user.</returns>
        /// <param name="username">Username.</param>
        Group GetInheritedGroupForUser(string username);

        #endregion
    }

    /// <summary>
    /// Expected permission cases
    /// </summary>
    public enum Permission : byte
    {
        Denied = 0,
        Permitted
    }

    /// <summary>
    /// Bare implementation for the required needs of a Data Connector
    /// </summary>
    public interface IDataConnector : IPermissionHandler
    {
        /// <summary>
        /// Gets the builder.
        /// </summary>
        /// <returns>The builder.</returns>
        /// <param name="pluginName">Plugin name.</param>
        QueryBuilder GetBuilder(string pluginName);

        //        QueryBuilder GetBuilder(string pluginName, string command, System.Data.CommandType type);

        /// <summary>
        /// Opens the connection to the data store
        /// </summary>
        void Open();

        /// <summary>
        /// Execute the specified builder.
        /// </summary>
        /// <param name="builder">Builder.</param>
        bool Execute(QueryBuilder builder);

        /// <summary>
        /// Executes the builder and returns the insert id.
        /// </summary>
        /// <returns>The insert.</returns>
        /// <param name="builder">Builder.</param>
        long ExecuteInsert(QueryBuilder builder);

        /// <summary>
        /// Executes the non query as specified in the builder
        /// </summary>
        /// <returns>The non query.</returns>
        /// <param name="builder">Builder.</param>
        int ExecuteNonQuery(QueryBuilder builder);

        /// <summary>
        /// Executes the a scalar query via the builder
        /// </summary>
        /// <returns>The scalar.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        T ExecuteScalar<T>(QueryBuilder builder);

        /// <summary>
        /// Executes the builder and returns the data set.
        /// </summary>
        /// <returns>The data set.</returns>
        /// <param name="builder">Builder.</param>
        DataSet ExecuteDataSet(QueryBuilder builder);

        /// <summary>
        /// Executes the buidler and returns an array of reflected rows
        /// </summary>
        /// <returns>The array.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        T[] ExecuteArray<T>(QueryBuilder builder) where T : new();
    }

    /// <summary>
    /// The bare implementation of a query builder
    /// </summary>
    public abstract class QueryBuilder : IDisposable
    {
        private string _plugin;
        private System.Text.StringBuilder _sb;
        private System.Data.CommandType _type;

        //Simple builder
        public QueryBuilder(string pluginName)
        {
            _sb = new System.Text.StringBuilder();

            _plugin = pluginName;
            _type = CommandType.Text;
        }

        //        //Command builder, essentially just for parameterised queries
        //        public QueryBuilder(string pluginName, string command, System.Data.CommandType type)
        //        {
        //            _sb = new System.Text.StringBuilder();
        //
        //            _sb.Append(command);
        //            _plugin = pluginName;
        //            System.Data.CommandType _type = type;
        //        }

        protected QueryBuilder Append(string fmt, params object[] args)
        {
            if (args == null || args.Length == 0)
                _sb.Append(fmt);
            else
                _sb.Append(String.Format(fmt, args));

            return this;
        }

        void IDisposable.Dispose()
        {
            if (_sb != null)
            {
                _sb.Clear();
                _sb = null;
            }
            _plugin = null;
        }

        /// <summary>
        /// Adds a parameter.
        /// </summary>
        /// <returns>The parameter.</returns>
        /// <param name="name">Name.</param>
        /// <param name="value">Value.</param>
        /// <param name="prefix">Prefix.</param>
        public abstract QueryBuilder AddParam(string name, object value, string prefix = "prm");

        /// <summary>
        /// Check if a table exists
        /// </summary>
        /// <returns>The exists.</returns>
        /// <param name="name">Name.</param>
        public abstract QueryBuilder TableExists(string name);

        /// <summary>
        /// Creates a table
        /// </summary>
        /// <returns>The create.</returns>
        /// <param name="name">Name.</param>
        /// <param name="columns">Columns.</param>
        public abstract QueryBuilder TableCreate(string name, params TableColumn[] columns);

        /// <summary>
        /// Drops a table
        /// </summary>
        /// <returns>The drop.</returns>
        /// <param name="name">Name.</param>
        public abstract QueryBuilder TableDrop(string name);

        //        public virtual QueryBuilder ProcedureExists(string name){
        //            return this.Append("select 1 from information_schema.routines where routine_type='procedure' and
        //        }
        //
        //        public abstract QueryBuilder ProcedureCreate(string name, string contents, params DataParameter[] parameters);
        //
        //        public abstract QueryBuilder ProcedureDrop(string name);

        //        public abstract QueryBuilder ExecuteProcedure(string name, string prefix = "prm", params DataParameter[] parameters);

        /// <summary>
        /// Begins a SELECT query
        /// </summary>
        /// <param name="expression">Expression.</param>
        public abstract QueryBuilder Select(params string[] expression);

        /// <summary>
        /// Adds the all expression to the query
        /// </summary>
        public abstract QueryBuilder All();

        /// <summary>
        /// Adds the from table selector
        /// </summary>
        /// <param name="tableName">Table name.</param>
        public abstract QueryBuilder From(string tableName);

        /// <summary>
        /// Adds a filter on data
        /// </summary>
        /// <param name="clause">Clause.</param>
        public abstract QueryBuilder Where(params WhereFilter[] clause);

        //        public abstract QueryBuilder WhereNotExists(QueryBuilder bld);
        //
        //        public abstract QueryBuilder WhereExists(QueryBuilder bld);

        /// <summary>
        /// Adds the count expression
        /// </summary>
        /// <param name="expression">Expression.</param>
        public abstract QueryBuilder Count(string expression = null);

        /// <summary>
        /// Add a DELETE statement
        /// </summary>
        public abstract QueryBuilder Delete();

        /// <summary>
        /// Build a DELETE statement
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="clause">Clause.</param>
        public virtual QueryBuilder Delete(string tableName, params WhereFilter[] clause)
        {
            if (null == clause || clause.Length == 0)
                return this.Delete().From(tableName);
            return this.Delete().From(tableName).Where(clause);
        }

        /// <summary>
        /// Add an INSERT TO statement
        /// </summary>
        /// <returns>The into.</returns>
        /// <param name="tableName">Table name.</param>
        /// <param name="values">Values.</param>
        public abstract QueryBuilder InsertInto(string tableName, params DataParameter[] values);

        /// <summary>
        /// Adds a UPDATE statement with specified columns and values
        /// </summary>
        /// <returns>The values.</returns>
        /// <param name="tableName">Table name.</param>
        /// <param name="values">Values.</param>
        public abstract QueryBuilder UpdateValues(string tableName, DataParameter[] values);

        /// <summary>
        /// Builds an UPDATE query
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="values">Values.</param>
        /// <param name="clause">Clause.</param>
        public virtual QueryBuilder Update(string tableName, DataParameter[] values, params WhereFilter[] clause)
        {
            if (null == clause || clause.Length == 0)
                return this.UpdateValues(tableName, values);
            return this.UpdateValues(tableName, values).Where(clause);
        }

        /// <summary>
        /// Builds a SELECT ALL (*) query
        /// </summary>
        /// <returns>The all.</returns>
        /// <param name="tableName">Table name.</param>
        /// <param name="clause">Clause.</param>
        public virtual QueryBuilder SelectAll(string tableName, params WhereFilter[] clause)
        {
            if (null == clause || clause.Length == 0)
                return this.Select().All().From(tableName);
            return this.Select().All().From(tableName).Where(clause);
        }

        /// <summary>
        /// Builds a SELECT [EXPRESSION] FROM query
        /// </summary>
        /// <returns>The from.</returns>
        /// <param name="tableName">Table name.</param>
        /// <param name="expression">Expression.</param>
        /// <param name="clause">Clause.</param>
        public virtual QueryBuilder SelectFrom(string tableName, string[] expression = null, params WhereFilter[] clause)
        {
            if (null == clause || clause.Length == 0)
                return this.Select(expression).From(tableName);
            return this.Select(expression).From(tableName).Where(clause);
        }

        //public virtual QueryBuilder If()
        //{
        //    return this.Append("IF ");
        //}
        //public virtual QueryBuilder Not()
        //{
        //    return this.Append("NOT ");
        //}

        //public virtual QueryBuilder Exists()
        //{
        //    return this.Append("EXISTS ");
        //}

        //public virtual QueryBuilder Else()
        //{
        //    return this.Append("ELSE ");
        //}

        //public virtual QueryBuilder OpenBracket()
        //{
        //    return this.Append("( ");
        //}

        //public virtual QueryBuilder CloseBracket()
        //{
        //    return this.Append(") ");
        //}

        protected string GetObjectName(string name)
        {
            return _plugin + '_' + name;
        }

        /// <summary>
        /// Gets or sets the type of the command.
        /// </summary>
        /// <value>The type of the command.</value>
        public CommandType CommandType
        {
            get
            { return _type; }
            set
            { _type = value; }
        }

        /// <summary>
        /// Gets or sets the command text.
        /// </summary>
        /// <value>The command text.</value>
        public string CommandText
        {
            get
            { return _sb.ToString(); }
            set
            {
                _sb.Clear();
                _sb.Append(value);
            }
        }

        /// <summary>
        /// Builds the command.
        /// </summary>
        /// <returns>The command.</returns>
        public virtual string BuildCommand()
        {
            return _sb.ToString();
        }
    }

    /// <summary>
    /// Data parameter
    /// </summary>
    public struct DataParameter
    {
        public string Name { get; set; }

        public object Value { get; set; }

        public DataParameter(string name, object value)
            : this()
        {
            this.Name = name;
            this.Value = value;
        }
    }

    /// <summary>
    /// Table column.
    /// </summary>
    public struct TableColumn
    {
        public string Name { get; set; }

        public object DefaultValue { get; set; }

        public Type DataType { get; set; }

        public bool AutoIncrement { get; set; }

        public bool PrimaryKey { get; set; }

        public bool Nullable { get; set; }

        public int? MinScale { get; set; }

        public int? MaxScale { get; set; }

        public TableColumn(string name, Type dataType, bool autoIncrement, bool primaryKey, bool allowNulls = false)
            : this()
        {
            this.Name = name;
            this.DefaultValue = null;
            this.DataType = dataType;
            this.AutoIncrement = autoIncrement;
            this.PrimaryKey = primaryKey;
            this.Nullable = allowNulls;
            this.MinScale = null;
            this.MaxScale = null;
        }

        public TableColumn(string name, Type dataType, bool allowNulls = false)
            : this()
        {
            this.Name = name;
            this.DefaultValue = null;
            this.DataType = dataType;
            this.AutoIncrement = false;
            this.PrimaryKey = false;
            this.Nullable = allowNulls;
            this.MinScale = null;
            this.MaxScale = null;
        }

        public TableColumn(string name, Type dataType, int scale, bool allowNulls = false)
            : this()
        {
            this.Name = name;
            this.DefaultValue = null;
            this.DataType = dataType;
            this.AutoIncrement = false;
            this.PrimaryKey = false;
            this.Nullable = allowNulls;
            this.MinScale = scale;
            this.MaxScale = null;
        }
    }

    /// <summary>
    /// Where filter.
    /// </summary>
    public struct WhereFilter
    {
        public string Column { get; set; }

        public object Value { get; set; }

        public WhereExpression Expression { get; set; }

        public WhereFilter(string column, object value, WhereExpression expression = WhereExpression.EqualTo)
            : this()
        {
            this.Expression = expression;
            this.Column = column;
            this.Value = value;
        }
    }

    /// <summary>
    /// Where expression.
    /// </summary>
    public enum WhereExpression : byte
    {
        EqualTo,
        Like,
        NotEqualTo
    }

    /// <summary>
    /// Direct access to the active Data Connector.
    /// </summary>
    /// <remarks>Plugins use this</remarks>
    public static class Storage
    {
        private static readonly object _sync = new object();
        private static IDataConnector _connector;

        /// <summary>
        /// Gets a value indicating if there is a connector available.
        /// </summary>
        /// <value><c>true</c> if is available; otherwise, <c>false</c>.</value>
        public static bool IsAvailable
        {
            get
            { return _connector != null; }
        }

        /// <summary>
        /// Sets the active connector.
        /// </summary>
        /// <param name="connector">Connector.</param>
        /// <param name="throwWhenSet">If set to <c>true</c> and a connector has already been set an exception will be thrown.</param>
        public static void SetConnector(IDataConnector connector, bool throwWhenSet = true)
        {
            lock (_sync)
            {
                if (_connector != null && throwWhenSet)
                {
                    throw new InvalidOperationException(String.Format("Attempted to load '{0}' when a '{1}' was already loaded", connector.ToString(), _connector.ToString()));
                }
                _connector = connector;
            }

            AuthenticatedUsers.Initialise();
            SettingsStore.Initialise();
        }

        /// <summary>
        /// Gets a builder compatible with the connector
        /// </summary>
        /// <returns>The builder.</returns>
        /// <param name="pluginName">Calling plugin name for encapsulation.</param>
        public static QueryBuilder GetBuilder(string pluginName)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.GetBuilder(pluginName);
        }

        //        public static QueryBuilder GetBuilder(string pluginName, string command, System.Data.CommandType type)
        //        {
        //            if (_connector == null)
        //                throw new InvalidOperationException("No connector attached");
        //            return _connector.GetBuilder(pluginName, command, type);
        //        }

        /// <summary>
        /// Execute the specified builder.
        /// </summary>
        /// <param name="builder">Builder.</param>
        public static bool Execute(QueryBuilder builder)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.Execute(builder);
        }

        /// <summary>
        /// Executes the builder and returns an insert id.
        /// </summary>
        /// <returns>The insert.</returns>
        /// <param name="builder">Builder.</param>
        public static long ExecuteInsert(QueryBuilder builder)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.ExecuteInsert(builder);
        }

        /// <summary>
        /// Executes a non query.
        /// </summary>
        /// <returns>The non query.</returns>
        /// <param name="builder">Builder.</param>
        public static int ExecuteNonQuery(QueryBuilder builder)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.ExecuteNonQuery(builder);
        }

        /// <summary>
        /// Executes the builder and returns the first row and column as a value.
        /// </summary>
        /// <returns>The scalar.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T ExecuteScalar<T>(QueryBuilder builder)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.ExecuteScalar<T>(builder);
        }

        /// <summary>
        /// Executes and returns a data set.
        /// </summary>
        /// <returns>The data set.</returns>
        /// <param name="builder">Builder.</param>
        public static DataSet ExecuteDataSet(QueryBuilder builder)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.ExecuteDataSet(builder);
        }

        /// <summary>
        /// Executes and reflects rows into an array.
        /// </summary>
        /// <returns>The array.</returns>
        /// <param name="builder">Builder.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static T[] ExecuteArray<T>(QueryBuilder builder) where T : new()
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.ExecuteArray<T>(builder);
        }

        /// <summary>
        /// Determines if a player is permitted for a node
        /// </summary>
        /// <returns><c>true</c> if is permitted the specified node player; otherwise, <c>false</c>.</returns>
        /// <param name="node">Node.</param>
        /// <param name="player">Player.</param>
        public static Permission IsPermitted(string node, BasePlayer player)
        {
            if (_connector == null)
                return player.Op ? Permission.Permitted : Permission.Denied;
            return _connector.IsPermitted(node, player);
        }

        /// <summary>
        /// Find a group by name
        /// </summary>
        /// <returns>The group.</returns>
        /// <param name="name">Name.</param>
        public static Group FindGroup(string name)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.FindGroup(name);
        }

        /// <summary>
        /// Add or update a group
        /// </summary>
        /// <returns><c>true</c>, if the update group was added/updated, <c>false</c> otherwise.</returns>
        /// <param name="name">Name.</param>
        /// <param name="applyToGuests">If set to <c>true</c>, the group will be applied to guests.</param>
        /// <param name="parent">Parent.</param>
        /// <param name="r">The red chat component.</param>
        /// <param name="g">The green chat component.</param>
        /// <param name="b">The blue chat component.</param>
        /// <param name="prefix">Prefix.</param>
        /// <param name="suffix">Suffix.</param>
        public static bool AddOrUpdateGroup(string name, bool applyToGuests = false, string parent = null, byte r = 255, byte g = 255, byte b = 255, string prefix = null, string suffix = null)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.AddOrUpdateGroup(name, applyToGuests, parent, r, g, b, prefix, suffix);
        }

        /// <summary>
        /// Remove a group
        /// </summary>
        /// <returns><c>true</c>, if group was removed, <c>false</c> otherwise.</returns>
        /// <param name="name">Name.</param>
        public static bool RemoveGroup(string name)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.RemoveGroup(name);
        }

        /// <summary>
        /// Adds a node to a group
        /// </summary>
        /// <returns><c>true</c>, if group node was added, <c>false</c> otherwise.</returns>
        /// <param name="groupName">Group name.</param>
        /// <param name="node">Node.</param>
        /// <param name="deny">If set to <c>true</c> deny.</param>
        public static bool AddGroupNode(string groupName, string node, bool deny = false)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.AddGroupNode(groupName, node, deny);
        }

        /// <summary>
        /// Removes a node from a group
        /// </summary>
        /// <returns><c>true</c>, if group node was removed, <c>false</c> otherwise.</returns>
        /// <param name="groupName">Group name.</param>
        /// <param name="node">Node.</param>
        /// <param name="deny">If set to <c>true</c> deny.</param>
        public static bool RemoveGroupNode(string groupName, string node, bool deny = false)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.RemoveGroupNode(groupName, node, deny);
        }

        /// <summary>
        /// Fetches the group names available
        /// </summary>
        /// <returns>The list.</returns>
        public static string[] GroupList()
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.GroupList();
        }

        /// <summary>
        /// Fetches the nodes for a group
        /// </summary>
        /// <returns>The nodes.</returns>
        /// <param name="groupName">Group name.</param>
        public static PermissionNode[] GroupNodes(string groupName)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.GroupNodes(groupName);
        }

        /// <summary>
        /// Adds a user to a group
        /// </summary>
        /// <returns><c>true</c>, if user to group was added, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        /// <param name="groupName">Group name.</param>
        public static bool AddUserToGroup(string username, string groupName)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.AddUserToGroup(username, groupName);
        }

        /// <summary>
        /// Removes a player from a group
        /// </summary>
        /// <returns><c>true</c>, if user from group was removed, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        /// <param name="groupName">Group name.</param>
        public static bool RemoveUserFromGroup(string username, string groupName)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.RemoveUserFromGroup(username, groupName);
        }

        /// <summary>
        /// Adds a specific node to a user
        /// </summary>
        /// <returns><c>true</c>, if node to user was added, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        /// <param name="node">Node.</param>
        /// <param name="deny">If set to <c>true</c> deny.</param>
        public static bool AddNodeToUser(string username, string node, bool deny = false)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.AddNodeToUser(username, node, deny);
        }

        /// <summary>
        /// Removes a specific node from a user
        /// </summary>
        /// <returns><c>true</c>, if node from user was removed, <c>false</c> otherwise.</returns>
        /// <param name="username">Username.</param>
        /// <param name="node">Node.</param>
        /// <param name="deny">If set to <c>true</c> deny.</param>
        public static bool RemoveNodeFromUser(string username, string node, bool deny = false)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.RemoveNodeFromUser(username, node, deny);
        }

        /// <summary>
        /// Fetches the associated groups names for a user
        /// </summary>
        /// <returns>The group list.</returns>
        /// <param name="username">Username.</param>
        public static string[] UserGroupList(string username)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.UserGroupList(username);
        }

        /// <summary>
        /// Fetches the list of nodes for a user
        /// </summary>
        /// <returns>The nodes.</returns>
        /// <param name="username">Username.</param>
        public static PermissionNode[] UserNodes(string username)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.UserNodes(username);
        }

        /// <summary>
        /// Fetches the lower most group for a player
        /// </summary>
        /// <remarks>There should always be one at this stage in OTA. The flexibility is just here.</remarks>
        /// <returns>The inherited group for user.</returns>
        /// <param name="username">Username.</param>
        public static Group GetInheritedGroupForUser(string username)
        {
            if (_connector == null)
                throw new InvalidOperationException("No connector attached");
            return _connector.GetInheritedGroupForUser(username);
        }
    }
}

