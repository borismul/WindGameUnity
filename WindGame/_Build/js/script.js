function anotherFunction()
{
	var canvas = document.getElementById("canvas");
	canvas.style.visibility = "hidden";
	// var div;

	// // document.getElementById("lectures").style.visibility = "visible";
	// // document.getElementById("lectures").style.width = "100%";
	// // document.getElementById("lectures").style.height = "100%";
	// // var object = document.createElement("DIV");
	// // object.innerHTML = '<object type="text/html" data="presentation.html" ></object>';
	// // document.body.appendChild(object);
	// // if(document.getElementById('presentation') == null){
	// 	div = document.createElement("div");	
	// 	div.setAttribute("w3-include-html", "presentation.html");
	// 	div.id = 'presentation';
	// 	document.body.appendChild(div);
	// 	div.style.width = "100%;"
	// 	div.style.height ="90%";


	// 	// var aButton = document.createElement("BUTTON");
	// 	// div.appendChild(aButton);
	// 	// aButton.innerHTML = "Button";
	// 	// aButton.onclick = unhide;
	// 	w3IncludeHTML();
	// }	
	// else{
		var  	div = document.getElementById('presentation');
		div.style.width = "100%;"
		div.style.height ="90%";
		div.style.visibility = "visible";
	// }

	// loadScript("head.min.js", doNothing);
	// alert("SHould be done");
	myPrettyCode();

}

function unhide()
{
	document.getElementById("canvas").style.visibility = "visible";
	document.getElementById('presentation').style.visibility = 'hidden';
	document.getElementById('presentation').style.height = 0;
}

function loadScript(url, callback)
{
    // Adding the script tag to the head as suggested before
    var head = document.getElementsByTagName('head')[0];
    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.src = url;

    // Then bind the event to the callback function.
    // There are several events for cross browser compatibility.
    // script.onreadystatechange = callback;
    script.onload = callback;

    // Fire the loading
    head.appendChild(script);
}

var doNothing = function()
{
	// loadScript("js/reveal.js", myPrettyCode);
}

var myPrettyCode = function() {

   // Here, do what ever you want
   // More info https://github.com/hakimel/reveal.js#configuration
	Reveal.initialize({
	  history: true,

	  // More info https://github.com/hakimel/reveal.js#dependencies
	  dependencies: [
	    { src: 'plugin/markdown/marked.js' },
	    { src: 'plugin/markdown/markdown.js' },
	    { src: 'plugin/notes/notes.js', async: true },
	    { src: 'plugin/highlight/highlight.js', async: true, callback: function() { hljs.initHighlightingOnLoad(); } }
	  ]
	});
};
