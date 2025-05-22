
using DevCodeArchitect.Utilities;
using static DevCodeArchitect.Entity.Types;

namespace DevCodeArchitect.Entity;

public class UserEnum
{
    // prepare settings for front-end application
    public static object getSettings()
    {
        return new
        {
            // property settings
            types = getTypes(),
        };
    }

    public static List<KeyValuePair<string, int>> getTypes()
    {
        var _enumList = ((AgentTypes[])Enum.GetValues(typeof(AgentTypes)))
            .Select(e => new KeyValuePair<string, int>(e.ToString(), (int)e))
            .ToList();
        return UtilityHelper.EnumOrderBy(_enumList);
    }

    // Service Providers
    public enum AgentTypes
    {
        Dealer = 1,
        Builder = 2,
        Electrician = 3,
        Plumber = 4,
        All = 5
    }
       
    public enum ActionType
    {
        Management = 0, // internal users e.g admins, moderators that have roles
        NonManagement = 1 // external users, logged in users, company users, agent, service provider etc
    }

    public enum UserTypes
    {
        NormalUser = 0,
        ServiceProvider = 1,
        Administrator = 2,
    }

}

/* Copyright © 2025, Mediasoftpro All rights reserved.
 * For inquiries and more information, please contact us at:
 * Email: clouddevarchitect@outlook.com
 * Website: www.devcodearchitect.com
 */