console.log('JS Test script active');

var onUpdate = OTAPIRuntime.On.Terraria.Main.Update.connect(function (orig, instance, gameTime) {
    // console.log('Update');
    orig(instance, gameTime);
});

var Dispose = () => {
    console.log('JS Disposing');
    onUpdate.disconnect();
};