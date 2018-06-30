namespace Hamwic.Cif.Core.Constants
{
    public static class RoleNames
    {
        /// <summary>
        /// All system functions
        /// </summary>
        public const string SystemAdministrator = "SystemAdministrator";

        /// <summary>
        /// Access to view, edit , delete all records across all schools
        /// </summary>
        public const string HamwicUser = "Hamwic.CifUser";

        /// <summary>
        /// Access to view, edit and delete records for one or more schools
        /// </summary>
        public const string ExecutiveHeadTeacher = "ExecutiveHeadTeacher";

        /// <summary>
        /// Access to view, edit and delete records for one school
        /// </summary>
        public const string HeadTeacher = "HeadTeacher";

        /// <summary>
        /// Access to view, edit and delete their own records only
        /// </summary>
        public const string SchoolUser = "SchoolUser";
    }
}