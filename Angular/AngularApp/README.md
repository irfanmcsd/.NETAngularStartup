
# Command for publishing application

ng build --base-href=/ --deploy-url /app/account/ --output-hashing none

# Deploy in root of application

ng build --base-href=/ --deploy-url / --output-hashing none

# Note - You must have registered account and .NET Application must run before runing angular app.

# Open index.html and update api url (.NET App Url e.g https://localhost:44353) and user id (e.g f2c4174b-b200-4fdb-b882-0d40d2c05ed8 [must exist])
