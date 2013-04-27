# Create your views here.
from django.shortcuts import render_to_response, redirect, render
from django.template import RequestContext

def test(request):
    return render_to_response("test.html", RequestContext(request))

def home(request):
	if request.user.is_authenticated():
		return render_to_response("home.html", RequestContext(request))
	else:
		return render_to_response("register_login.html", RequestContext(request))