## README

This README would normally document whatever steps are necessary to get the
application up and running.

Things you may want to cover:

*   Ruby version

*   System dependencies

*   Configuration

*   Database creation

*   Database initialization

*   How to run the test suite

*   Services (job queues, cache servers, search engines, etc.)


EMAIL infrastructure: using sendgrid (please set up an account for
development) and add the SENDGRID_USERNAME and SENDGRID_PASSWORD to your
config/application.yml file

*   Continuous Integration


	Temporarily using [CodeShip](https://codeship.com/projects/86389) to build
and deploy to Heroku. 	If you need access to it send your github account email
to pmap@wallem.com

*   Deployment


It's automatically deployed to Staging/UAT/QA after merging branches into git
staging branch. 

It's on Heroku stack.

*   Staging/UAT/QA:
    [nobita.herokuapp.com/](https://nobita.herokuapp.com
    /)
*   Production: to be arranged
*   Production web api: to be arranged


Please feel free to use a different markup language if you do not plan to run
`rake doc:app`.
