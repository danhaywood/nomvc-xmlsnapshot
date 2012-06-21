## What is this? ##

It's a demonstrator of using the NakedObjects XML Snapshot service, eg in the context of its use within recorded actions.

It also demonstrates a custom view and a custom controller.

Note that this example does *not* demonstrate recorded action clusters.

## Entities ##

- Claim, which has a 1:m to
- Allowance
- RecordedActionForClaim


## What to look at ##

- create a claim
- add some allowances
- make a decision (eg pending)
  - this creates a recorded action as a side effect
- list the recorded actions of the claim
- view one of these recorded actions
  - it has a custom view with a button to "View Snapshot"
- use the button to view the XML
  - this hits the custom controller
