
namespace ForLab.Data.Enums
{
    #region Application
    public enum ApplicationRolesEnum
    {
        SuperAdmin = 1,
        CountryLevel = 2,
        RegionLevel = 3,
        LaboratoryLevel = 4,
        ViewOnlyLevel = 5
    }
    public enum UserSubscriptionLevelsEnum
    {
        CountryLevel = 1,
        RegionLevel = 2,
        LaboratoryLevel = 3,
        ViewOnlyLevel = 4
    }
    public enum UserStatusEnum
    {
        Active = 1,
        NotActive = 2,
        Locked = 3
    }
    public enum UserTransactionTypesEnum
    {
        NameChanging = 1,
        IsActiveChanging = 2,
        EmailChanging = 3,
        PasswordChanging = 4,
        PhoneChanging = 5,
        AddressChanging = 6,
        AccountLock = 7,
        Login = 8,
        Logout = 9,
        ForgetPassword = 10,
        ResetPassword = 11,
    }
    public enum ActionOfAuditEnum
    {
        Create = 1,
        Update = 2,
        Activate = 3,
        Deactivate = 4,
        CreateFromDuplicate = 5
    }
    #endregion

    #region Static Lookups
    public enum CalculationPeriodEnum
    {
        OneYear = 1,
        TwoYears = 2,
    }
    public enum ContinentEnum
    {
        Asia = 1,
        Africa = 2,
        NorthAmerica = 3,
        SouthAmerica = 4,
        Antarctica = 5, 
        Europe = 6,
        Australia = 7
    }
    public enum ControlRequirementUnitEnum
    {
        Daily = 1,
        Weekly = 2,
        Monthly = 3
    }
    public enum CountryPeriodEnum
    {
        Weekly = 1,
        Monthly = 2,
        Quarterly = 3,
        Annualy = 4
    }
    public enum EntityTypeEnum
    {
    }
    public enum ForecastConsumableUsagePeriodEnum
    {
        Daily = 1,
        Weekly = 2,
        Monthly = 3,
        Quarterly = 4,
        Annualy = 5
    }
    public enum ForecastInfoLevelEnum
    {
        Country = 1,
        Region = 2,
        Laboratory = 3
    }
    public enum ForecastInfoStatusEnum
    {
        Open = 1,
        Closed = 2
    }
    public enum ForecastMethodologyEnum
    {
        Service = 1,
        Consumption = 2,
        DempgraphicMorbidity = 3
    }
    public enum ProductBasicUnitEnum
    {
        Box = 1,
        Each = 2,
        Kit = 3,
        Vial = 4,
        L = 5,
        ml = 6,
        Pack = 7,
        Piece = 8,
        Pipette = 9,
        Roll = 10,
        Test = 11,
        Tip = 12,
        Tube = 13
    }
    public enum ProductTypeEnum
    {
        Reagents = 1,
        QualityControl = 2,
        Consumables = 3,
        Durables = 4,
        Calibrators = 5
    }
    public enum ReagentSystemEnum
    {
        Open = 1,
        Closed = 2,
        PartiallyOpen = 3
    }
    public enum ScopeOfTheForecastEnum
    {
        National = 1,
        Global = 2,
        ProgramBased = 3
    }
    public enum ThroughPutUnitEnum
    {
        Hourly = 1,
        Daily = 2,
    }
    public enum VariableTypeEnum
    {
        Numeric = 1,
        Percentage = 2
    }
    #endregion

    #region General
    public enum DaysOfWeek
    {
        Saturday,
        Sunday,
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday
    }
    #endregion
}
