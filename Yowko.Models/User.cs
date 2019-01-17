using System;

namespace Yowko.Models
{
    public interface IConvertType
    {
    }
    [AttributeUsage(AttributeTargets.Property)]
    public class JsonTypeAttribute : Attribute
    {
    }
    public class User : IConvertType
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public string Addr { get; set; }
        [JsonType]
        private TitleModel Titles { get; set; }
        #region - 以下為 json string 中的屬性 -
        public string OrgName
        {
            get { return Titles.OrgName; }
            set { Titles.OrgName = value; }
        }
        public string Title
        {
            get { return Titles.Title; }
            set { Titles.Title = value; }
        }
        public string Award
        {
            get { return Titles.Award; }
            set { Titles.Award = value; }
        }
        public decimal Salary
        {
            get { return Titles.Salary; }
            set { Titles.Salary = value; }
        }
        #endregion
    }
    public class TitleModel
    {
        public string OrgName { get; set; }
        public string Title { get; set; }
        public string Award { get; set; }
        public decimal Salary { get; set; }
    }
}
