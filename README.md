## OkToBoard

This README would normally document whatever steps are necessary to get the application up and running.

Things you may want to cover:

* Ruby version
* System dependencies
* Configuration
* Database creation
* Database initialization
* How to run the test suite
* Services (job queues, cache servers, search engines, etc.)

## Preparation

### Ruby version: 2.1.6

### Rails: 4.1.12

### System dependencies
OkToBoard Web service, is server that is hosted on Wallem's network. We assume that the webservice is available already. For more detail, please see **Deployment** section in below.

### Email configuration
Using sendgrid (please set up an account for development) and add the SENDGRID_USERNAME and SENDGRID_PASSWORD to your config/application.yml file

### Database: Postgres

### Data initialization:

Create tables
`rake db:migrate`

Initial data
`rake db:seed`

Synchronize cruise calls from CrewChange system to OTB database
`rake app:sync:ccs_data`

[Optional]For development envinronment, to generate fake OkToBoard requests:
`rake db:data:fake`

### Run background job:
`foreman start`

### Run test suite
`rake spec`

## Continuous Integration
Using [CircleCI](https://circleci.com/gh/wallem-group/ok_to_board) to build and deploy to Heroku. If you need access to it send your github account email to pmap@wallem.com

## Deployment

It's automatically deployed to Staging/UAT/QA after merging branches into git staging branch. 

It's on Heroku stack.

### Staging/UAT/QA:
* Web application:
    [staging-oktoboard.herokuapp.com/](https://staging-oktoboard.herokuapp.com/)
* Web API endpoint: 
    [http://203.129.81.16:7000/api](http://203.129.81.16:7000/api)

### Production
* Web application: 
    [okboard.wallem.com/](http://okboard.wallem.com/)
* Web API endpoint:
    [https://ws-okboard.wallem.com/api/](https://ws-okboard.wallem.com/api/)

### Web API methods:
    Get vessels: 
      /shipname

    Get ETA by vessel: 
      /getetabyship/<vessel_id>
      Url paramenter:
          vessel_id: ID of vessel in GUID format

    Generate report: 
      /report?id=<id>&report_type=<type>
      Url parameters: 
          id: ID of OkToBoard request
          report_type: type of report (pdf or doc)

## License

Ruby on Rails is released under the [MIT License](http://www.opensource.org/licenses/MIT).
