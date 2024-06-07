 ### Approach
  - Save every date into UTC
  - Allow user to switch time zone
 ### Filtering Approach 
  - First Convert the payload date into the local user timezone
  - Second convert, Converted local time to UTC
  - Boom, Ready for the filter.
  
### Filtering Approach without user preferences
  - Change the date to UTC. before filtering

### Date display in UI
  - convert UTC to local date-time based on user preference.
  
