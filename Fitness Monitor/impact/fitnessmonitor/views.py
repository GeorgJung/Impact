# Create your views here.
from django.shortcuts import render_to_response, redirect, render
from django.template import RequestContext

def test(request):
    return render_to_response("test.html", RequestContext(request))