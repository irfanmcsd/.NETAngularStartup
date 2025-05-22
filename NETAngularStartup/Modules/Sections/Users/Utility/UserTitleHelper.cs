using DevCodeArchitect.Entity;
using DevCodeArchitect.Utilities;

namespace DevCodeArchitect.SDK;
public class UserTitleHelper
{
    public static void Generate(UserListViewModel listentity, UserListingQueryModel entity)
    {
        var _title = "Agents";
      

        // search
        if (!string.IsNullOrEmpty(entity.Term))
        {
            // Property Search in "{term}" => Property Search in "Commercial Apartments in Ohio"
            _title = "Agents Search in " + UtilityHelper.SetTitle(entity.Term);
        }

        // company listings
        if (!string.IsNullOrEmpty(entity.CompanySlug))
        {
            // {company_slug} Property Listings
            _title = UtilityHelper.SetTitle(entity.CompanySlug) + " Agents";
        }


        if (!string.IsNullOrEmpty(entity.PlaceTerm))
        {
            // Property Listings in {place_term}
            _title = "Agents in " + UtilityHelper.SetTitle(entity.PlaceTerm);
        }
        else if (!string.IsNullOrEmpty(entity.State) && !string.IsNullOrEmpty(entity.Country))
        {
            // Property Listings in Texas, US
            _title = "Agents in " + UtilityHelper.SetTitle(entity.State) + ", " + UtilityHelper.SetTitle(entity.Country);
        }
        else if (!string.IsNullOrEmpty(entity.Country))
        {
            // Property Listings in Texas, US
            _title = "Agents in " + UtilityHelper.SetTitle(entity.Country);
        }

        // Featured 
        if (entity.Featured != null)
        {
            switch (entity.Featured)
            {
                case Types.FeaturedTypes.Featured:
                    _title = "Featured Agents";
                    break;
                
            }

        }


        listentity.HeadingTitle = _title;
    }

}