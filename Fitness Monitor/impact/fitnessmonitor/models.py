from django.db import models

# Create your models here.
class Practitioner(User):
    

class Session(models.Model):
	trainee = models.ForeignKey(Practitioner)
    start = models.DateTimeField(null=True)
    end = models.DateTimeField(null=True)

class Round(models.Model):
	trainee = models.ForeignKey(Practitioner)
    start = models.DateTimeField(null=True)
    end = models.DateTimeField(null=True)