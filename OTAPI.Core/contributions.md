#Contribution recommendations
		
####Callbacks
**Wrapping comparisions**: When wrapping a function using some of the OTAPI extensions for comparing, make sure the parameters of your callback match the same case as the
function being wrapped. 
You can make corrections to parameters in your hook handler :+1:

####Hooks
**Argument order**: When adding hooks ensure the handler arguments are in a sensible order, e.g. instance, arguments as defined in the vanilla function
This is so people can learn and remember that (typically) the first parameters are more important than the rest

**Argument alterations**: Tip, fair chance if you want your value typed variables to be altered via a hook handler you will need to pass them by reference using the ref keyword.
You might also notice that a lot of hook callbacks generate ref keywords using the ldloca opcode. This is how we alter some vanilla functionality.
		
Note: these tips may are being added as development progresses.